import { useState } from "react";
import { SearchBar } from "../components/SearchBar";
import { useNavigate } from "react-router-dom";
import Footer from "../components/Footer";

export default function HomePage() {
  const [query, setQuery] = useState("");
  const navigate = useNavigate();

  const handleSearch = (value: string) => {
    navigate(`/search?q=${encodeURIComponent(value)}`);
  };

  const suggestedSearches = [
    { term: "AI Innovations", category: "Tech" },
    { term: "Web3 News", category: "Blockchain" },
    { term: "Visiom Updates", category: "Platform" },
    { term: "Crypto Markets", category: "Finance" },
  ];

  const trendingSearches = [
    "Decentralized Apps",
    "NFT Trends 2025",
    "Smart Contracts",
    "Blockchain Security",
    "Metaverse News",
  ];

  const handleSuggestedSearch = (term: string) => {
    setQuery(term);
    navigate(`/search?q=${encodeURIComponent(term)}`);
  };

  return (
    <div className="flex flex-col items-center justify-center min-h-screen bg-gradient-to-b from-black to-neutral-900 text-white px-4 sm:px-6 lg:px-8">
      <div className="text-center mb-12 animate-fade-in">
        <h1 className="text-5xl sm:text-6xl font-bold tracking-tight mb-4 bg-clip-text text-transparent bg-gradient-to-r from-white to-neutral-400">
          visiom smart<span className="text-neutral-400">search</span>
        </h1>
        <p className="text-neutral-400 text-sm sm:text-base max-w-md mx-auto">
          Your modern, secure & open source search engine.
        </p>
      </div>

      <SearchBar
        value={query}
        onChange={setQuery}
        onSearch={handleSearch}
        view="regular"
        autoFocus
        className="w-full max-w-xl sm:max-w-2xl backdrop-blur-sm transition-all duration-300"
      />

      <div className="mt-12 w-full max-w-xl sm:max-w-2xl">
        <div className="mb-8">
          <h2 className="text-sm font-medium text-neutral-300 mb-4">Suggested Searches</h2>
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
            {suggestedSearches.map((search, index) => (
              <button
                key={index}
                onClick={() => handleSuggestedSearch(search.term)}
                className="flex items-center gap-2 p-3 bg-neutral-800/50 border border-neutral-700/50 rounded-lg hover:bg-neutral-700/70 hover:border-neutral-600 transition-all duration-200 ease-in-out backdrop-blur-sm text-left text-sm text-neutral-200 group"
              >
                <span className="text-neutral-400 text-xs bg-neutral-900/80 px-2 py-1 rounded-md group-hover:bg-neutral-800/90 transition-colors">
                  {search.category}
                </span>
                <span>{search.term}</span>
              </button>
            ))}
          </div>
        </div>

        <div>
          <h2 className="text-sm font-medium text-neutral-300 mb-4">Trending on Visiom</h2>
          <div className="flex flex-wrap gap-2">
            {trendingSearches.map((term, index) => (
              <button
                key={index}
                onClick={() => handleSuggestedSearch(term)}
                className="px-3 py-1.5 bg-neutral-900/80 border border-neutral-700/50 rounded-full text-xs text-neutral-300 hover:bg-neutral-700/70 hover:border-neutral-600 hover:text-white transition-all duration-200 ease-in-out backdrop-blur-sm"
              >
                {term}
              </button>
            ))}
          </div>
        </div>
      </div>

      <Footer />
    </div>
  );
}