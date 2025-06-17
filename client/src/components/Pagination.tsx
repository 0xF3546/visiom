import { motion } from "framer-motion";
import { IoIosArrowBack, IoIosArrowForward } from "react-icons/io";

export const Pagination = ({ currentPage, totalPages, onPageChange }: { 
  currentPage: number, 
  totalPages: number, 
  onPageChange: (page: number) => void 
}) => {
  const pagesToShow = 5;
  const getPageNumbers = () => {
    const pages = [];
    const startPage = Math.max(1, currentPage - Math.floor(pagesToShow / 2));
    const endPage = Math.min(totalPages, startPage + pagesToShow - 1);

    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    return pages;
  };

  return (
    <motion.div 
      className="flex gap-2 mt-6 justify-center"
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.4 }}
    >
      <button
        onClick={() => onPageChange(Math.max(1, currentPage - 1))}
        disabled={currentPage === 1}
        className={`px-3 py-1 rounded-md text-sm ${
          currentPage === 1 
            ? 'bg-neutral-800 text-neutral-500 cursor-not-allowed' 
            : 'bg-neutral-700 text-white hover:bg-neutral-600'
        }`}
      >
        <IoIosArrowBack />
      </button>

      {getPageNumbers().map((page) => (
        <button
          key={page}
          onClick={() => onPageChange(page)}
          className={`px-3 py-1 rounded-md text-sm ${
            currentPage === page
              ? 'bg-white text-black font-semibold'
              : 'bg-neutral-700 text-white hover:bg-neutral-600'
          }`}
        >
          {page}
        </button>
      ))}

      <button
        onClick={() => onPageChange(Math.min(totalPages, currentPage + 1))}
        disabled={currentPage === totalPages}
        className={`px-3 py-1 rounded-md text-sm ${
          currentPage === totalPages
            ? 'bg-neutral-800 text-neutral-500 cursor-not-allowed'
            : 'bg-neutral-700 text-white hover:bg-neutral-600'
        }`}
      >
        <IoIosArrowForward />
      </button>
    </motion.div>
  );
};