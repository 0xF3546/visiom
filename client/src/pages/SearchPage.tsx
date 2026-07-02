import { useSearchParams, useNavigate, Link } from "react-router-dom";
import { useEffect, useState } from "react";
import { SearchBar } from "../components/SearchBar";
import { AppRoutes } from "../AppRouter";
import { motion } from "framer-motion";
import Footer from "../components/Footer";
import {
  useSearchServiceGetApiSearch,
  useSearchServiceGetApiSearchImages,
} from "../generated/api/queries";
import { Pagination } from "../components/Pagination";
import Loader from "./Loader";

const tabs = ["All", "Images"] as const;
type Tab = (typeof tabs)[number];

const PAGE_SIZE = 12;

export default function SearchPage() {
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const query = params.get("q") || "";
  const page = Math.max(1, parseInt(params.get("page") || "1", 10));
  const [activeTab, setActiveTab] = useState<Tab>("All");
  const [searchQuery, setSearchQuery] = useState(query);

  useEffect(() => {
    setSearchQuery(query);
  }, [query]);

  const web = useSearchServiceGetApiSearch(
    { q: query, page, pageSize: PAGE_SIZE },
    undefined,
    { enabled: activeTab === "All" && query.trim().length > 0 }
  );
  const images = useSearchServiceGetApiSearchImages(
    { q: query, page, pageSize: PAGE_SIZE },
    undefined,
    { enabled: activeTab === "Images" && query.trim().length > 0 }
  );

  const active = activeTab === "All" ? web : images;
  const result = active.data;
  const isLoading = active.isLoading;
  const totalPages = Math.max(1, Math.ceil((result?.count || 0) / PAGE_SIZE));

  const goTo = (nextQuery: string, nextPage: number) =>
    navigate(`/search?q=${encodeURIComponent(nextQuery)}&page=${nextPage}`);

  const handleSearch = (value: string) => {
    if (value.trim() === "") return;
    goTo(value, 1);
  };

  const handlePageChange = (newPage: number) => goTo(query, newPage);

  const handleTabChange = (tab: Tab) => {
    setActiveTab(tab);
    if (page !== 1) goTo(query, 1); // pagination is per-tab; restart at page 1
  };

  const renderWeb = () => (
    <motion.ul
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
      className="space-y-4"
    >
      {web.data?.items?.map((r, idx) => (
        <li
          key={`${r.url}-${idx}`}
          className="bg-neutral-900/80 border border-neutral-700/50 rounded-lg p-4 sm:p-5 hover:bg-neutral-800/80 hover:border-neutral-600 transition-all duration-200 ease-in-out backdrop-blur-sm"
        >
          <a
            href={r.url}
            target="_blank"
            rel="noreferrer"
            className="text-white text-base sm:text-lg font-semibold hover:underline"
          >
            {r.title}
          </a>
          <p className="text-neutral-400 mt-1 sm:mt-2 text-sm line-clamp-2">
            {r.description}
          </p>
          <a
            href={r.url}
            target="_blank"
            rel="noreferrer"
            className="text-neutral-500 text-xs sm:text-sm mt-2 block hover:text-neutral-300 transition-colors truncate"
          >
            {r.url}
          </a>
        </li>
      ))}
    </motion.ul>
  );

  const renderImages = () => (
    <motion.div
      initial={{ opacity: 0, y: 10 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.4, ease: "easeOut" }}
      className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-3 sm:gap-4"
    >
      {images.data?.items?.map((img, idx) => (
        <a
          key={`${img.imageUrl}-${idx}`}
          href={img.sourcePageUrl ?? img.imageUrl}
          target="_blank"
          rel="noreferrer"
          title={img.title ?? ""}
          className="group relative block overflow-hidden rounded-lg border border-neutral-700/50 bg-neutral-900/80 aspect-square"
        >
          <img
            src={img.thumbnailUrl ?? img.imageUrl}
            alt={img.title ?? ""}
            loading="lazy"
            referrerPolicy="no-referrer"
            className="h-full w-full object-cover transition-transform duration-300 group-hover:scale-105"
            onError={(e) => {
              (e.currentTarget.parentElement as HTMLElement).style.display = "none";
            }}
          />
          {img.title && (
            <span className="absolute inset-x-0 bottom-0 truncate bg-black/60 px-2 py-1 text-[11px] text-neutral-200 opacity-0 transition-opacity group-hover:opacity-100">
              {img.title}
            </span>
          )}
        </a>
      ))}
    </motion.div>
  );

  const hasResults = (result?.items?.length ?? 0) > 0;

  return (
    <div className="flex flex-col min-h-screen bg-gradient-to-b from-black to-neutral-900 text-white font-satoshi">
      <header className="sticky top-0 z-50 bg-black/80 backdrop-blur-sm px-6 sm:px-8 py-4 flex items-center gap-6 sm:gap-8">
        <Link
          to={AppRoutes.HOME.path}
          className="text-2xl sm:text-3xl font-bold tracking-tight bg-clip-text text-transparent bg-gradient-to-r from-white to-neutral-400 hover:text-neutral-300 transition-colors"
        >
          visiom
        </Link>
        <div className="flex-1 max-w-2xl sm:max-w-3xl">
          <SearchBar
            value={searchQuery}
            onSearch={handleSearch}
            onChange={(e) => setSearchQuery(e)}
            view="compact"
            className="w-full backdrop-blur-sm transition-all duration-300 mt-2"
            autoFocus
          />
        </div>
      </header>

      <div className="flex-1 px-6 sm:px-8 pt-8 sm:pt-10">
        <nav className="flex space-x-6 sm:space-x-8 mb-6">
          {tabs.map((tab) => (
            <button
              key={tab}
              onClick={() => handleTabChange(tab)}
              className={`pb-2 text-sm sm:text-base font-medium transition-colors relative ${
                activeTab === tab
                  ? "text-white font-semibold"
                  : "text-neutral-400 hover:text-neutral-200"
              }`}
            >
              {tab}
              {activeTab === tab && (
                <motion.div
                  layoutId="tab-indicator"
                  className="absolute bottom-0 left-0 w-full h-0.5 bg-gradient-to-r from-white to-neutral-400"
                  transition={{ type: "spring", stiffness: 500, damping: 30 }}
                />
              )}
            </button>
          ))}
        </nav>

        <main className="max-w-4xl mx-auto">
          <h1 className="text-neutral-400 text-sm sm:text-base mb-6">
            Results for{" "}
            <span className="text-white font-semibold">"{query}"</span> in{" "}
            <span className="text-white font-semibold">{activeTab}</span>
          </h1>

          {isLoading ? (
            <motion.div
              animate={{ opacity: [0.5, 1, 0.5] }}
              transition={{ repeat: Infinity, duration: 1.5 }}
              className="text-neutral-400 text-sm sm:text-base"
            >
              <Loader
                text={activeTab === "Images" ? "Searching images..." : "Searching..."}
              />
            </motion.div>
          ) : hasResults ? (
            <>
              {activeTab === "All" ? renderWeb() : renderImages()}
              {totalPages > 1 && (
                <Pagination
                  currentPage={page}
                  totalPages={totalPages}
                  onPageChange={handlePageChange}
                />
              )}
            </>
          ) : (
            <motion.p
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.4 }}
              className="text-neutral-400 text-sm sm:text-base"
            >
              No results found for "{query}". The index is still growing — the
              background crawler may not have reached this topic yet.
            </motion.p>
          )}
        </main>
      </div>

      <div className="flex flex-col items-center justify-center mb-6">
        <Footer />
      </div>
    </div>
  );
}
