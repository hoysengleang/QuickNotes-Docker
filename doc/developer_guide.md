# QuickNotes Developer Guide 🚀

## 1. Project Overview
QuickNotes is a modern, containerized note-taking application designed for speed, security, and ease of use. It features a responsive Vue.js frontend, a robust .NET 8 backend, and SQL Server for data persistence, all orchestrated via Docker.

### Key Features
- **User Authentication**: Secure Login & Register with BCrypt hashing and Terms/Privacy consent.
- **Note Management**: Create, Edit, Pin, Color-code, Delete (Soft & Hard), and Restore notes.
- **Search & Filter**: Real-time search and filtering (All, Trash).
- **Admin System**: dedicated admin view for monitoring system error logs.
- **Responsive Design**: Mobile-first UI using Tailwind CSS.

---

## 2. Technical Stack
- **Frontend**: Vue 3 (Composition API), Vite, Tailwind CSS, TypeScript.
- **Backend**: ASP.NET Core 8 Web API, Dapper (ORM).
- **Database**: SQL Server 2022.
- **Caching**: Redis (for Note caching).
- **Testing**: xUnit, Moq.
- **Infrastructure**: Docker & Docker Compose.

---

## 3. Getting Started

### Prerequisites
- Docker & Docker Compose
- .NET 8 SDK (for local dev/testing)
- Node.js 18+ (for local frontend dev)

### Installation & Running
The entire stack is containerized. To start the application:

```bash
# Build and start all services
sudo docker compose up --build
```

Access the application at:
- **Frontend**: http://localhost:3000
- **Backend API**: http://localhost:8080/swagger

---

## 4. Business Flows & User Journey

### 4.1. Authentication Flow
1.  **Registration**:
    - User visits `/register`.
    - Enters Username, Password (strong validation), and confirms password.
    - **MUST** agree to Terms of Service & Privacy Policy (viewable via modal).
    - Backend creates user with hashed password.
2.  **Login**:
    - User visits `/login`.
    - Enters credentials -> specific validation feedback.
    - On success, user object is stored in `localStorage` and user is redirected to Dashboard.
3.  **Security**:
    - Passwords are never stored in plain text.
    - Admin users have an `IsAdmin` flag.

### 4.2. Note Lifecycle
1.  **Creation**: User clicks "Take a note...", enters title/content, picks a color -> `POST /api/notes`.
2.  **View**: Notes are displayed in a Masonry-like grid or list. Pinned notes appear at the top.
3.  **Edit**: Clicking a note opens a modal. Autosave or "Done" triggers `PUT /api/notes/{id}`.
4.  **Delete**:
    - **Soft Delete**: "Trash" icon moves note to Trash (`IsDeleted = 1`).
    - **Hard Delete**: In Trash, "Delete Forever" removes from DB.
5.  **Restore**: In Trash, "Restore" brings note back to active list.

### 4.3. Admin System
- **Access**: Only users with `IsAdmin = true` key sees "Admin Logs" in sidebar.
- **Function**: View application error logs (StackTrace, Message) captured by the global error handler.

---

## 5. Project Structure

### Backend (`/api`)
- **Controllers/**: API Endpoints (`AuthController`, `NotesController`).
- **Repositories/**: Data access layer (`NoteRepository`, `AuthRepository`). Uses Dapper for SQL.
- **Models/**: C# DTOs and Logic classes (`Note`, `User`).
- **Program.cs**: Dependency Injection, DB connection, and Middleware setup.

### Frontend (`/client`)
- **src/views/**: Page-level components (`HomeView`, `LoginView`, `RegisterView`).
- **src/components/**: Reusable UI inputs (`NoteCard`, `Sidebar`, `LegalModal`).
- **src/stores/**: Pinia state management (`noteStore.ts` handles API calls).

---

## 6. Development Workflow

### Making Backend Changes
1.  Modify `Controller` or `Repository`.
2.  If creating new logic, define Interface (`IRepository`) first.
3.  Updates to DB Schema require updating `Program.cs` (Migration logic is embedded there on startup).

### Running Unit Tests
We use xUnit and Moq.
```bash
dotnet test api.tests/api.tests.csproj
```

### Making Frontend Changes
1.  Edit Vue components.
2.  Tailwind classes handles styling.
3.  If adding new API calls, add them to `noteStore.ts` first.

---

## 7. Troubleshooting
- **White Screen / API Error**: Check Docker logs `docker compose logs -f notes_api`.
- **Database Connection**: Ensure SQL Server container is healthy. The API automatically retries connection on startup.
- **"Unknown User"**: Ensure `localStorage` has valid JSON. Clear storage and re-login.

---

