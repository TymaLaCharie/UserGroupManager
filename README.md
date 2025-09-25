User Group Manager

User Group Manager is a full-stack web application built with .NET 8, demonstrating a modern, maintainable, and scalable architecture for managing users, groups, and permissions. The project features a secure RESTful API backend and a dynamic Blazor Server frontend, emphasizing a clear separation of concerns, robust database design, and a permission-based authorization system.

## Table of Contents

- Core Features
- Technical Stack
- Architecture
- Key Concepts & Logic
  - First User Registration: Automatic Admin Promotion
  - Permission-Based Authorization
  - Approval Workflow
- Getting Started: How to Run the Project
  - Prerequisites
  - Configuration
  - Database Setup
  - Running the Application
- Project Structure
- API Enpoints

## Core Features

- **User Management**: Secure CRUD (Create, Read, Update, Delete) operations for users.
- **Group & Permission Management**: Administrators can create groups and assign granular permissions to them.
- **Approval Workflow**: New user registrations require administrator approval before they can log in.
- **Role-Based Access Control**: A clear distinction between **Standard Users** and **Administrators**, with the UI dynamically adapting to the user's permissions.
- **Dashboard & Statistics**: A modern, card-based dashboard displaying key application metrics like total users and users per group.
- **Secure Authentication**: Uses JSON Web Tokens (JWT) for secure, stateless authentication between the frontend and backend.
- **User-Friendly Admin Console**: Dedicated UI for administrators to approve new users, manage groups, and assign permissions.

## Technical Stack

- **Backend**: C#, .NET 8, ASP.NET Core Web API
- **Frontend**: Blazor Server (.NET 8)
- **Database**: SQL Server (Code-First approach)
- **ORM**: Entity Framework Core 8
- **Authentication**: JWT Bearer Tokens
- **Password Encryption**: BCrypt.Net

## Architecture

The solution is structured using a clean, layered architecture to promote separation of concerns, testability, and maintainability.

-   **`UserGroupManager.Domain`**: The core of the application. Contains the business entities (User, Group, Permission) and enums, with no external dependencies.
-   **`UserGroupManager.Infrastructure`**: Handles data access and external services. It contains the Entity Framework `DbContext`, database migrations, and repository implementations.
-   **`UserGroupManager.Api`**: The ASP.NET Core Web API project. It exposes the application's functionality via RESTful endpoints and handles authentication and authorization for external clients.
-   **`UserGroupManager.Web`**: The Blazor Server frontend project. It provides the user interface and consumes the backend API.

## Key Concepts & Logic

This section details important business logic built into the application.

### First User Registration: Automatic Admin Promotion

**The first person to register an account in the system is automatically granted full administrative privileges.** This is a critical bootstrapping feature that ensures the system has an administrator from the very beginning.

Here's how it works:
1.  When a new user submits their registration, the API checks if there are any existing users in the database.
2.  **If the database is empty**, the new user is:
    -   Marked as `IsAdmin = true`.
    -   Given an `Approved` status, allowing them to log in immediately.
    -   Automatically added to the "Administrators" group, granting them all associated permissions.
3.  **All subsequent users** who register will have a default status of `Pending` and will not be admins. They cannot log in until an existing administrator approves their account via the "User Approvals" page in the Admin Console.

### Permission-Based Authorization

The application uses a powerful permission-based system, not just simple roles.
-   **Permissions** are the most granular actions (e.g., `Manage Users`, `View Reports`).
-   **Groups** are collections of permissions. A user is assigned to one or more groups.
-   A user's effective permissions are the unique sum of all permissions granted by the groups they are a member of.
-   When a user logs in, these permissions are encoded into their JWT. The UI and API then check for the presence of a specific permission (e.g., `"CanManageGroups"`) to grant or deny access to features and data.

### Approval Workflow

-   **New Users**: Must be approved by an admin in the "User Approvals" section.
-   **Group Membership**: An admin can add or remove users from groups via the "Manage Groups" page for a specific user. All changes made by an admin are automatically approved.

## Getting Started: How to Run the Project

Follow these steps to get the application running on your local machine.

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
-   SQL Server (e.g., SQL Server Express, SQL Server Developer Edition, or LocalDB installed with Visual Studio).
-   An IDE like Visual Studio 2022 or JetBrains Rider, or a code editor like VS Code.

### Configuration

1.  Clone the repository: `git clone https://github.com/your-username/UserGroupManager.git`
2.  Navigate to the API project's settings file: `src/UserGroupManager.Api/appsettings.json`.
3.  **Update the Connection String**: Modify the `SQLConnection` string to point to your local SQL Server instance. The default uses LocalDB.
4.  **Check API Port**: Open `src/UserGroupManager.Api/Properties/launchSettings.json` and note the `applicationUrl` (e.g., `https://localhost:7123`).
5.  **Update Blazor App**: Open `src/UserGroupManager.Web/Program.cs` and ensure the `BaseAddress` for the `HttpClient` matches the API's port.

### Database Setup

The project uses EF Core's Code-First migrations. To create and seed the database:
1.  Open a terminal or command prompt in the root directory of the solution.
2.  Run the following command:
    dotnet ef database update --project src/UserGroupManager.Infrastructure/ --startup-project src/UserGroupManager.Api/
 1. 
   This will create a database named `UserGroupManagerDB_Final` (or as configured) and seed it with initial groups and permissions.

### Running the Application

You need to run both the backend API and the frontend Blazor app simultaneously.

1.  **Run the API:**
    dotnet run --project src/UserGroupManager.Api

2.  **Open a new terminal** and run the Blazor Web App:
    ```bash
    dotnet run --project src/UserGroupManager.Web
    ```
3.  Open your browser and navigate to the Blazor application's URL (e.g., `https://localhost:7228`).
4.  You will be redirected to the login page. Click "Register" to create the first user, who will become the administrator.

## Project Structure