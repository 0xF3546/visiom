import { useSearchParams, useNavigate, Link } from "react-router-dom";
import { useEffect, useState } from "react";
import { SearchBar } from "../components/SearchBar";
import { AppRoutes } from "../AppRouter";
import { motion } from "framer-motion";
import Footer from "../components/Footer";
import { useSearchServiceGetApiSearch } from "../generated/api/queries";
import { Pagination } from "../components/Pagination";
import Loader from "./Loader";

const tabs = ["All", "Images", "News"];

export default function SearchPage() {
  const [params] = useSearchParams();
  const navigate = useNavigate();
  const query = params.get("q") || '';
  const page = parseInt(params.get("page") || "1", 10);
  const [activeTab, setActiveTab] = useState("All");
  const [searchQuery, setSearchQuery] = useState(query);

  const { 
    data: result,
    isLoading
  } = useSearchServiceGetApiSearch({
    q: query,
    page: page,
    pageSize: 12,
  });

  const handleSearch = (value: string) => {
    if (value.trim() === "") return;
    navigate(`/search?q=${encodeURIComponent(value)}&page=1`);
  };

  const handlePageChange = (newPage: number) => {
    navigate(`/search?q=${encodeURIComponent(query)}&page=${newPage}`);
  };

  useEffect(() => {
    if (page !== 1) {
      navigate(`/search?q=${encodeURIComponent(query)}&page=1`);
    }
  }, [query, activeTab, navigate]);

  const resultList = () => {
    if (!result || !result.items || result.items.length === 0) {
      return (
        <motion.p
          initial={{ opacity: 0 }}
          animate={{ opacity: 1 }}
          transition={{ duration: 0.4 }}
          className="text-neutral-400 text-sm sm:text-base"
        >
          No results found for "{query}".
        </motion.p>
      );
    }
    return (
      <motion.ul
        initial={{ opacity: 0, y: 10 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.4, ease: "easeOut" }}
        className="space-y-4"
      >
        {result?.items.map((result, idx) => (
          <li
            key={idx}
            className="bg-neutral-900/80 border border-neutral-700/50 rounded-lg p-4 sm:p-5 hover:bg-neutral-800/80 hover:border-neutral-600 transition-all duration-200 ease-in-out backdrop-blur-sm"
          >
            <a
              href={result.url}
              className="text-white text-base sm:text-lg font-semibold hover:underline"
            >
              {result.title}
            </a>
            <p className="text-neutral-400 mt-1 sm:mt-2 text-sm line-clamp-2">
              {result.description}
            </p>
            <a
              href={result.url}
              className="text-neutral-500 text-xs sm:text-sm mt-2 block hover:text-neutral-300 transition-colors"
            >
              {result.url}
            </a>
          </li>
        ))}
      </motion.ul>
    );
  };

  // Calculate total pages (using count as in provided code)
  const totalPages = Math.ceil((result?.count || 0) / 12);

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
              onClick={() => setActiveTab(tab)}
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
          {!isLoading  && (
            <h1 className="text-neutral-400 text-sm sm:text-base mb-6">
              Results for <span className="text-white font-semibold">"{query}"</span> in{" "}
              <span className="text-white font-semibold">{activeTab}</span>
            </h1>
          )}

          {isLoading ? (
            <motion.div
              animate={{ opacity: [0.5, 1, 0.5] }}
              transition={{ repeat: Infinity, duration: 1.5 }}
              className="text-neutral-400 text-sm sm:text-base"
            >
              <Loader text="Searching web..." />
            </motion.div>
          ) : (result?.items?.length ?? 0) > 0 ? (
            <>
              {resultList()}              
                <Pagination
                  currentPage={page}
                  totalPages={totalPages}
                  onPageChange={handlePageChange}
                />
            </>
          ) : (
            <motion.p
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 0.4 }}
              className="text-neutral-400 text-sm sm:text-base"
            >
              No results found for "{query}".
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