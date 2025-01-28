# Library Management System

This Library Management System provides RESTful APIs to manage books, members, and their transactions (borrow/return). It includes a console application for interacting with the APIs and demonstrating core functionality.

---

## Instructions on Running the Project

### Prerequisites
- **.NET SDK**: Ensure you have .NET SDK 9.0 or higher installed.
- **IDE/Editor**: Use Visual Studio, Visual Studio Code, or any other .NET-compatible IDE.

### Steps to Run

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/tudormarc/LibraryManagement.git
   cd LibraryManagement
   ```

2. **Build the Solution**:
   ```bash
   dotnet build
   ```

3. **Run the API**:
   Navigate to LibraryManagement and run:
   ```bash
   cd LibraryManagement
   export ASPNETCORE_ENVIRONMENT=Development
   dotnet run
   ```
   This will start the API at `http://localhost:5124`.

4. **Run the Console Application**:
   Navigate to LibraryManagementConsole and execute:
   ```bash
   cd LibraryManagementConsole
   dotnet run
   ```

   Follow the on-screen menu to interact with the system.

4. **Check swagger documentation page**
  ``` http://localhost:5124/swagger ```
---

## Description of Implemented Features

### 1. **Manage Books**
- Add, update, delete, and retrieve books.
- Search books by title, author, or category.
- View all books currently borrowed by a member.

### 2. **Manage Members**
- Add, update, delete, and retrieve members.

### 3. **Borrow and Return Books**
- Members can borrow up to 5 books at a time.
- Books are marked unavailable upon borrowing.
- Tracks due dates (14 days by default).
- Prevents borrowing if overdue books exist.

### 4. **Overdue Books**
- Lists all overdue books based on the current date.

### 5. **Console Application**
- Provides an interactive interface for:
  - Adding books and members.
  - Borrowing and returning books.
  - Searching books by criteria.
  - Viewing books borrowed by a specific member.
  - Displaying overdue books.

---

## Assumptions and Limitations

### Assumptions
- The system assumes the server runs at `http://localhost:5124`.
- Each book has a unique ID, title, author, and category.
- Members are uniquely identified by their IDs.

### Limitations
- **In-Memory Database**:
  - The system uses an in-memory database for simplicity, and data will reset on application restart.
  
- **No Authentication**:
  - The system does not include user authentication or role-based access control.

- **Logging**:
  - A logging mechanism is not implemented yet

- **Notifications**:
  - Overdue notifications are logged only and do not integrate with email or messaging services.

- **Scalability**:
  - Not optimized for large-scale deployment; intended for educational purposes.

---

Feel free to modify the project to add additional features or adapt it for production use.

