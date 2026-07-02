namespace visiom.Core.Index
{
    internal static class SqliteSchema
    {
        public const string Ddl = @"
            CREATE TABLE IF NOT EXISTS pages(
                id INTEGER PRIMARY KEY,
                url TEXT UNIQUE NOT NULL,
                title TEXT,
                content TEXT,
                crawled_at TEXT
            );

            CREATE VIRTUAL TABLE IF NOT EXISTS pages_fts USING fts5(
                title, content,
                content='pages', content_rowid='id',
                tokenize='unicode61'
            );

            CREATE TRIGGER IF NOT EXISTS pages_ai AFTER INSERT ON pages BEGIN
                INSERT INTO pages_fts(rowid, title, content) VALUES (new.id, new.title, new.content);
            END;
            CREATE TRIGGER IF NOT EXISTS pages_ad AFTER DELETE ON pages BEGIN
                INSERT INTO pages_fts(pages_fts, rowid, title, content) VALUES('delete', old.id, old.title, old.content);
            END;
            CREATE TRIGGER IF NOT EXISTS pages_au AFTER UPDATE ON pages BEGIN
                INSERT INTO pages_fts(pages_fts, rowid, title, content) VALUES('delete', old.id, old.title, old.content);
                INSERT INTO pages_fts(rowid, title, content) VALUES (new.id, new.title, new.content);
            END;

            CREATE TABLE IF NOT EXISTS images(
                id INTEGER PRIMARY KEY,
                page_id INTEGER NOT NULL,
                image_url TEXT UNIQUE NOT NULL,
                alt TEXT,
                source_page_url TEXT,
                width INTEGER,
                height INTEGER,
                crawled_at TEXT
            );
            CREATE INDEX IF NOT EXISTS ix_images_page ON images(page_id);";
    }
}
