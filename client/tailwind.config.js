/** @type {import('tailwindcss').Config} */
module.exports = {
  mode: 'jit',
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        satoshi: ['"Satoshi"', "ui-sans-serif", "system-ui"],
      },
      colors: {
        primaryBlue: "#2563EB",
        darkBg: "#0F172A",
        darkBgSecondary: "#1E293B",
        textPrimary: "#CBD5E1",
        textSecondary: "#94A3B8",
      },
    },
  },

  plugins: [],
}