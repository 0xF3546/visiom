# Persistent-Index Search Engine — Design

**Date:** 2026-07-01
**Status:** Approved (decisions confirmed with user)

## Problem

The crawler live-crawls the web on every query, seeding from URLs derived from the
query. The user wants a real search-engine architecture: a background crawler that
continuously builds an index, and queries that hit only the index — no per-query
crawling or seeding.

## Decisions (confirmed)

- **Index storage:** SQLite + FTS5 (embedded, no server, BM25 full-text ranking).
- **Crawl operation:** a background `IHostedService` that auto-crawls on startup and
  runs continuously (politeness + robots.txt, bounded).
- **Bootstrap seeds:** a small configurable set of start URLs in `appsettings.json`.
  This is a one-time bootstrap of the whole crawl, **not** per-query seeding.

## Accepted trade-off

A query only returns what has already been crawled. The index starts empty and fills
over time; early queries may be sparse. This is inherent to the index model.

## Architecture

```
CrawlerHostedService (background, N workers)
   frontier(queue) + visited + per-domain politeness + robots
        │  fetch+parse via visiom.Crawler.Crawler.FetchAsync(url)
        ▼
   ISearchIndex.IndexPageAsync(page)          →  SQLite (pages, pages_fts, images)
                                                        ▲
SearchController → ICrawlerService (query)  ── FTS MATCH + bm25 ─┘
        (offset pagination, real total count)
```

### Components

- **visiom.Crawler** — reduced to a pure fetcher. `Crawler.FetchAsync(string url)`
  returns a `CrawledPageDto` (url, title, content, links, images). DTOs made public.
  All query/seed/match/snippet logic removed (ranking now lives in the index).

- **visiom.Core / Index** — `ISearchIndex` + `SqliteSearchIndex`:
  - `InitializeAsync()` — create schema (WAL mode, FTS5 tables + sync triggers).
  - `IsIndexedAsync(url)` — dedup check.
  - `IndexPageAsync(CrawledPageDto)` — upsert page + its images.
  - `SearchPagesAsync(query, skip, take)` → `(items, total)` via `pages_fts MATCH`,
    ordered by `bm25`, with `snippet()` for descriptions.
  - `SearchImagesAsync(query, skip, take)` → images joined to matching pages.
  - Single writer (the hosted service); readers use WAL concurrent reads.

- **visiom.Core / Crawling** — `CrawlerHostedService : BackgroundService` +
  `CrawlerOptions` (bound from `"Crawler"`). Owns the frontier, per-domain delay,
  robots.txt cache, page/domain caps; writes to `ISearchIndex`.

- **visiom.Core / WebCrawler** — `ICrawlerService.CrawlAsync/CrawlImagesAsync` now
  query the index with **offset** pagination (Page/PageSize) and return real totals.

- **visiom.Api** — register `ISearchIndex` (singleton), `CrawlerOptions`, and the
  hosted service in `Startup`. Endpoints drop `nextPageToken` (offset now).

- **client** — regenerate API client; restore numeric `Pagination` (component already
  exists) for web + images using the real `count`. Tabs: All + Images.

### Config (`appsettings.json`)

```json
"Crawler": {
  "Seeds": ["https://en.wikipedia.org/wiki/Main_Page"],
  "IndexPath": "visiom-index.db",
  "MaxIndexedPages": 5000,
  "MaxPagesPerDomain": 200,
  "RequestDelayMsPerDomain": 1000,
  "Workers": 4
}
```

### SQLite schema (essentials)

```sql
CREATE TABLE pages(id INTEGER PRIMARY KEY, url TEXT UNIQUE, title TEXT, content TEXT, crawled_at TEXT);
CREATE VIRTUAL TABLE pages_fts USING fts5(title, content, content='pages', content_rowid='id');
-- AI/AD/AU triggers keep pages_fts in sync
CREATE TABLE images(id INTEGER PRIMARY KEY, page_id INTEGER, image_url TEXT UNIQUE,
                    alt TEXT, source_page_url TEXT, width INT, height INT, crawled_at TEXT);
```

FTS MATCH input is sanitized: split into alnum tokens, quote each, join with ` AND `.

## Out of scope (early cut)

Persistent frontier (in-memory, rebuilt from seeds; indexed pages skipped on restart),
distributed/decentralized crawling, ranking beyond BM25, incremental re-crawl scheduling.
