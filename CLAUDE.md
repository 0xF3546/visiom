# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

Visiom is intended to be a **decentralized, privacy-first search engine**. Privacy and
decentralization are the product's reason to exist — weigh architectural decisions against that goal
(don't centralize or leak user queries).

The current implementation is an early cut and only partially reflects that vision: a React/TypeScript
SPA (`client/`) talks to a .NET 8 Web API (`webapi/`). A background crawler continuously indexes the
web into a local SQLite full-text index; search queries hit **only** that index (no live per-query
crawl, so the query never leaves the backend). Still missing from the vision: decentralization and a
broader privacy layer. The index is bootstrapped from a small configurable seed list.

Component roles: `visiom.Crawler` = web crawler, `visiom.Api` = HTTP request handling,
`visiom.Core` = business logic, `client` = React frontend.

## Commands

Frontend (run from `client/`):
- `npm install` — install dependencies
- `npm run dev` — dev server at http://localhost:5173 (README's `npm start` does not exist)
- `npm run build` — type-check (`tsc -b`) then Vite build; this is what CI runs
- `npm run lint` — ESLint (flat config in `eslint.config.js`)
- `npm run codegen` — regenerate the API client (see "Generated API client" below)

There is **no test runner wired up** despite the client README mentioning Jest — `npm test`/`npm run test` will fail.

Backend (run from repo root):
- `dotnet build visiom.sln` — build all three backend projects
- `dotnet run --project webapi/src/visiom.Api` — run the API (Swagger UI at `/swagger` in Development)

There are **no backend tests** despite the README referencing a `/tests` folder.

## Architecture

### Backend — three-project chain (`webapi/src/`)
- **visiom.Api** — ASP.NET Core host. Uses the classic `Startup` class (`Configuration/Startup.cs`),
  not minimal-hosting/`WebApplication`. `SearchController` exposes `GET /api/Search` (web) and
  `GET /api/Search/images`. CORS origins come from the `CorsUrls` array in `appsettings.json`.
- **visiom.Core** — service/orchestration layer. `ConfigureCore(IConfiguration)` (in `Configuration.cs`)
  wires DI: binds `CrawlerOptions`, registers `ISearchIndex` (SQLite, singleton), `ICrawlerService`
  (query path), and the `CrawlerHostedService` background crawler. Holds the SQLite index
  (`Index/`), the crawl orchestration (`Crawling/`), and pagination helpers (`Extensions/`).
- **visiom.Crawler** — a **pure page fetcher** (`Crawler.cs`): `FetchAsync(url)` downloads+parses one
  page into title/content/links/images. Holds no crawl state. Depends on AngleSharp. (Newtonsoft.Json
  is no longer used by the crawl path.)

Dependency direction: `Api → Core → Crawler`. HTTP/DI in Api, crawl orchestration + index + query in
Core, pure fetching in Crawler.

### How search actually works (index-based)
- **Background crawler** (`visiom.Core/Crawling/`) runs for the app's lifetime. `CrawlerHostedService`
  orchestrates workers; the concerns are split out: `CrawlFrontier` (in-memory queue + dedup + per-domain
  caps), `DomainThrottle` (per-host politeness delay + serialization) and `RobotsCache` (robots.txt). It
  crawls from the bootstrap seeds (`Crawler:Seeds` in `appsettings.json`), bounded by `MaxIndexedPages`/
  `MaxPagesPerDomain`, and writes every fetched page + its images to the index. Already-indexed pages are
  skipped on restart, so the index persists and grows across runs.
- **Index** (`visiom.Core/Index/`) is SQLite + FTS5, split by concern: `SqliteConnectionFactory`,
  `SqliteSchema` (DDL), `FtsMatchQuery` (sanitizes input into a quoted-AND MATCH expression, so FTS
  operators can't be injected), and the write/read seam `ISearchIndexWriter`/`ISearchIndexReader`
  (`SqliteSearchIndexWriter`/`SqliteSearchIndexReader`). Pages live in `pages` with an external-content
  FTS table `pages_fts` (kept in sync by triggers); images in `images` (FK to page). Search uses
  `MATCH` + `bm25()` ranking and `snippet()` for descriptions. The DB file (`visiom-index.db`,
  gitignored) sits at the API content root. The crawler depends only on the *writer*, the query path
  only on the *reader*.
- **Query path** (`visiom.Core/WebCrawler/impl/CrawlerService.cs`) reads the index only — no live crawl.
- **Pagination is offset-based**: `PageResult<T>.Count` is the real total from the index, so numeric
  paging works (`Page`/`PageSize`). There is no `nextPageToken` anymore.

### Pagination contract (important, non-obvious)
`PageRequest` binds `Page`/`PageSize` from **HTTP headers** (`[FromHeader]`), not the query string.
The generated client honors this — `SearchService.getApiSearch`/`getApiSearchImages` send `Page`/`PageSize`
as headers and `q` as a query param. Keep this in sync if you change the endpoint signature.

### Frontend (`client/src/`)
- Vite + React 18 + TypeScript + Tailwind (via `@tailwindcss/vite`) + Framer Motion animations.
- Routing is **config-driven**: routes are declared in the `AppRoutes` object in `AppRouter.tsx`
  (each with `path`/`element`/`layout`/`auth`), which is then grouped by layout and rendered. Add
  pages by extending `AppRoutes`, not by hand-writing `<Route>`s. Auth-gated routes (`auth: true`)
  redirect through `AuthRoutes`; auth context/provider exist (`contexts/`, `hooks/useAuthProvider`)
  but there is currently no login page.
- Data fetching goes through TanStack Query hooks from the generated client. `pages/SearchPage.tsx`
  has two tabs — `All` (web, `useSearchServiceGetApiSearch`) and `Images` (grid,
  `useSearchServiceGetApiSearchImages`) — each enabled by the active tab and paged via the numeric
  `Pagination` component (offset paging off the real `count`). The `queryClient`
  (`utils/queryClien.ts` — note the filename typo) disables refetching and sets a 5-min staleTime.
- `main.tsx` installs an Axios request interceptor that attaches a `Bearer` token from
  `localStorage`, though search itself is unauthenticated.

### Generated API client (`client/src/generated/api/` - do not edit by hand)
- Generated by `@7nohe/openapi-react-query-codegen` + `@hey-api/openapi-ts` via `npm run codegen`,
  which reads the OpenAPI spec from **`client/public/api.json`**. Regenerate the spec from the running
  backend (NSwag serves it in Development) and drop it there before running codegen.
- The API base URL is **hardcoded** to `https://localhost:7295` in
  `generated/api/requests/core/OpenAPI.ts`. This is regenerated on codegen and does not read from env —
  verify it matches the backend's actual HTTPS port when search returns nothing.

## Gotchas
- All backend source lives under `webapi/src/` (`visiom.Api`, `visiom.Core`, `visiom.Crawler`).
- README's "Project Structure", ports (5000/7160), and `npm start` instructions are partly outdated;
  trust the actual files (frontend on 5173, backend HTTPS port per launch settings, client on 7295).
- CI (`.github/workflows/build-frontend.yml`) only lints (non-blocking) and builds the frontend on
  push/PR to `main`. The backend is not built or tested in CI.
