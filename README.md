# Microservices Project with .NET

This project is a microservices-based application built with .NET 8.0. Currently, it includes a User Service with JWT authentication.

## Prerequisites

- .NET 8.0 SDK
- Docker and Docker Compose
- Visual Studio Code (for debugging)

## Getting Started

1. Start the MySQL database:
   ```bash
   docker-compose up -d
   ```

2. Navigate to the UserService directory:
   ```bash
   cd src/UserService/UserService
   ```

3. Run the migrations:
   ```bash
   dotnet ef database update
   ```

4. Run the application:
   ```bash
   dotnet run
   ```

The application will start, and you can access the Swagger documentation at:
https://localhost:7172/swagger

## API Endpoints

### User Service (default port: 7172)

- POST /api/auth/register - Register a new user
- POST /api/auth/login - Login with existing credentials
- GET /api/auth/me - Get current user information (requires authentication)

## Debugging

The project includes launch configurations for VS Code. To debug:

1. Open the project in VS Code
2. Make sure the MySQL container is running
3. Press F5 or use the Run and Debug panel to start debugging

## Authentication

The service uses JWT Bearer token authentication. After logging in, include the token in the Authorization header:
```
Authorization: Bearer your-token-here
``` 