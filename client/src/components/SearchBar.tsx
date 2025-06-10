import React, { useState } from "react";

interface ISearchBarProps {
  value?: string;
  placeholder?: string;
  onChange?: (value: string) => void;
  onSearch?: (value: string) => void;
  className?: string;
  autoFocus?: boolean;
  view?: "regular" | "compact";
}

export const SearchBar = ({
  value,
  placeholder = "Search...",
  onChange,
  onSearch,
  className = "",
  autoFocus = false,
  view = "regular",
}: ISearchBarProps) => {
  const [internalValue, setInternalValue] = useState("");

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    if (onChange) onChange(newValue);
    if (value === undefined) setInternalValue(newValue);
  };

  const handleSearch = () => {
    if (onSearch) onSearch(value ?? internalValue);
  };

  const inputValue = value !== undefined ? value : internalValue;

  const viewClasses =
    view === "compact"
      ? "py-1.5 px-3 text-sm rounded-lg"
      : "py-2.5 px-5 text-base rounded-lg";

  return (
    <div className={`flex items-center gap-3 ${className}`}>
      <input
        type="text"
        placeholder={placeholder}
        value={inputValue}
        onChange={handleChange}
        onKeyDown={(e) => e.key === "Enter" && handleSearch()}
        className={`bg-neutral-900/80 text-white placeholder-neutral-500 border border-neutral-700/50 focus:outline-none focus:ring-2 focus:ring-neutral-500/50 backdrop-blur-sm ${viewClasses} flex-1 transition-all duration-300 ease-in-out hover:border-neutral-600 focus:shadow-lg focus:shadow-neutral-800/20`}
        aria-label="Search input"
        autoFocus={autoFocus}
      />
      <button
        onClick={handleSearch}
        className={`bg-white text-black font-medium hover:bg-neutral-200/90 active:scale-95 transition-all duration-200 ease-in-out ${viewClasses} shadow-sm hover:shadow-md hover:shadow-neutral-700/30`}
        aria-label="Search"
      >
        Search
      </button>
    </div>
  );
};