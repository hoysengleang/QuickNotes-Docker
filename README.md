<div align="center">

  <h1>📝 QuickNotes Docker</h1>
  
  <p>
    <strong>Containerized Note-Taking Solution</strong>
  </p>

  <p>
    <a href="#features">Features</a> •
    <a href="#tech-stack">Tech Stack</a> •
    <a href="#getting-started">Getting Started</a> •
    <a href="#api-documentation">API Docs</a>
  </p>

  <p>
    <img src="https://img.shields.io/badge/Vue.js-35495E?style=for-the-badge&logo=vuedotjs&logoColor=4FC08D" alt="Vue" />
    <img src="https://img.shields.io/badge/.NET%208-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 8" />
    <img src="https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white" alt="Docker" />
    <img src="https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white" alt="SQL Server" />
    <img src="https://img.shields.io/badge/Redis-DC382D?style=for-the-badge&logo=redis&logoColor=white" alt="Redis" />
  </p>
</div>

<br />

## 📖 About The Project

QuickNotes is a comprehensive, full-stack note-taking application designed for speed, security, and scalability. It leverages the power of **Docker** to provide a zero-configuration setup experience. Whether you're a developer looking for a reference architecture or a user needing a reliable notes app, QuickNotes has you covered.

## ✨ Key Features

- **🔐 Secure Authentication**: Robust user registration and login system with encrypted passwords and Terms of Service consent.
- **📝 Rich Note Management**: Create, update, delete, colored pins, and easy organization.
- **🗑️ Trash & Recovery**: Soft-delete functionality with a 7-day retention policy and restore options.
- **🔍 Smart Search**: Real-time filtering and search capabilities.
- **⚙️ Admin Dashboard**: Dedicated error logging and monitoring for system administrators.
- **📱 Responsive UI**: A mobile-first, beautiful interface built with Tailwind CSS and Vue 3.

---

## 🛠️ Tech Stack

| Component | Technology | Description |
|-----------|------------|-------------|
| **Frontend** | Vue 3, Vite, Tailwind | Fast, reactive, and beautiful UI. |
| **Backend** | ASP.NET Core 8 | High-performance Web API with clean architecture. |
| **Database** | SQL Server 2022 | Enterprise-grade relational database. |
| **Cache** | Redis | Distributed caching for blazing fast read speeds. |
| **Infrastructure** | Docker Compose | Orchardrates the entire stack with a single command. |

---

## 🚀 Getting Started

Since the entire application is containerized, you don't need to install .NET, Node.js, or SQL Server on your machine. You only need **Docker**.

### Prerequisites

*   [Docker Desktop](https://www.docker.com/products/docker-desktop) (Windows/Mac) or Docker Engine (Linux)

### Installation

1.  **Clone the repository**
    ```bash
    git clone https://github.com/yourusername/quicknotes-docker.git
    cd quicknotes-docker
    ```

2.  **Start the application**
    ```bash
    docker compose up --build
    ```
    *This command will build the frontend and backend images, pull SQL Server and Redis, and start everything.*

3.  **Access the App**
    *   **Frontend**: Open [http://localhost:3000](http://localhost:3000)
    *   **Backend API**: Running on [http://localhost:8080](http://localhost:8080)

---

## 📚 API Documentation

Once the application is running, you can explore the full REST API documentation via Swagger UI:

> **[http://localhost:8080/swagger](http://localhost:8080/swagger)**

---

## 🧪 Running Tests

To run the backend unit tests (requires .NET 8 SDK locally):

```bash
dotnet test api.tests/api.tests.csproj
```

---

## 📁 Project Structure

```
├── api/                # ASP.NET Core Web API
│   ├── Controllers/    # API Endpoints
│   ├── Models/         # Data Transformations
│   └── Repositories/   # Data Access Layer (Dapper)
├── client/             # Vue 3 Frontend
│   ├── src/views/      # Page Components
│   └── src/components/ # Reusable UI Components
├── api.tests/          # xUnit Test Project
├── doc/                # Developer Documentation
└── docker-compose.yml  # Container Orchestration
```

---

<div align="center">
  <p>Built with ❤️ by leangDev</p>
</div>
