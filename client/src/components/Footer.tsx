const Footer = () => {
    return (
        <footer className="mt-16 text-xs text-neutral-600">
        <p>
          Powered by Visiom &copy; 2025 |{" "}
          <a href="/privacy" className="hover:text-neutral-400 transition-colors">
            Privacy
          </a>{" "}
          |{" "}
          <a href="/terms" className="hover:text-neutral-400 transition-colors">
            Terms
          </a>
        </p>
      </footer>
    )
}

export default Footer;