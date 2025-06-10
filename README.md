# Visiom SmartSearch

![Visiom Homepage](/img/home.png)

**Visiom SmartSearch** is a modern, open-source search engine featuring a React frontend and a C# .NET backend. It delivers a sleek, user-friendly interface with a dark-themed, Web3-inspired design, emphasizing performance and aesthetics. Ideal for developers seeking a fast, customizable search solution with a focus on open-source collaboration.

## Features

- **Modern UI**: Glassmorphism design with smooth animations and a gradient-based dark theme.
- **Responsive Layout**: Optimized for mobile, tablet, and desktop using Tailwind CSS.
- **Open-Source**: Fully customizable, community-driven project under the MIT License.

## Installation

To run Visiom SmartSearch locally, follow these steps:

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/0xF3546/visiom_searchengine.git
   cd visiom_searchengine
   ```

2. **Set Up the Frontend**:
   Navigate to the React frontend:
   ```bash
   cd client
   npm install
   npm start
   ```
   The frontend will run at `http://localhost:5173`.

3. **Set Up the Backend**:
   Ensure you have [.NET SDK](https://dotnet.microsoft.com/download) installed. Navigate to the C# backend:
   ```bash
   cd ../webapi
   dotnet restore
   dotnet run
   ```
   The backend API will run (default: `http://localhost:5000` or `https://localhost:7160`).

## Project Structure
(TBD)
```
visiom_searchengine/
├── /client/                # React frontend
│   ├── /img/               # Static assets (e.g., home.png)
│   ├── /src/
│   │   ├── AppRouter.tsx   # Routing configuration
│   │   └── main.tsx        # Frontend entry point
│   ├── package.json        # Frontend dependencies
│   ├── tsconfig.json       # TypeScript configuration
├── /webapi/                # C# .NET backend
│   ├── /src/               # Source Code
│   ├── /tests/             # Tests
├── README.md               # Project documentation
```

## Contributing

We welcome contributions to Visiom SmartSearch! To contribute:

1. **Fork the Repository**:
   Click the Fork button on GitHub and submit a pull request to your repository.

2. **Create a Branch**:
   ```bash
   git checkout -b feature/your-feature-name
   ```

3. **Make Changes**:
   - For frontend: Update files in `/client/`.
   - For backend: Modify files in `/webapi/`.
   - Ensure code style (use Prettier for frontend, default .NET formatting for backend).

4. **Submit a Pull Request**:
   Push your changes to your fork and submit a PR to the `main` branch. Include a clear description of your changes.

Please follow the [Code of Conduct](CODE_OF_CONDUCT.md) and ensure all tests pass before submitting.

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

## Contact

For questions or feedback, open an issue on this repository or join the conversation in [GitHub Discussions](https://github.com/0xF3546/visiom_searchengine/discussions).

---
Powered by Visiom © 2025
