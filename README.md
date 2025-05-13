# MyBlog

A modern blog application built with .NET 9, implementing clean architecture principles.

## Project Structure

- **Domain**
  - **MyBlog.Core** - Contains domain entities, interfaces, and business logic
  - **MyBlog.Application** - Houses application services, commands, queries using MediatR
- **Infrastructure**
  - **MyBlog.Postgres** - Implements data access, external services
  - **MyBlog.Jwt** - JWT authentication/authorization components
- **Presentation**
  - **MyBlog.WebApi** - API endpoints and configuration

## Technologies

- .NET 9.0
- Entity Framework Core
- PostgreSQL
- MediatR for CQRS
- FluentValidation
- JWT Authentication
- Carter for minimal APIs

## Prerequisites

- .NET 9.0 SDK
- PostgreSQL database server
- Visual Studio 2022 or VS Code

## Getting Started

### Running Locally

1. Clone the repository

```sh
git clone https://github.com/ttdat2232/my-blog.git
cd ./my-blog
```

2. Update database connection string in `appsettings.json`

3. Run database migrations

```sh
dotnet ef database update -p ./Src/MyBlog.Postgres -s ./Src/MyBlog.WebApi
```

4. Run the application

```sh
dotnet run --project ./Src/MyBlog.WebApi
```

### Running with Docker

1. Clone the repository and navigate to the project directory

```sh
git clone https://github.com/ttdat2232/my-blog.git
cd ./my-blog
```

2. Build and run using Docker Compose

```sh
docker-compose up --build
```

This will start:

- The MyBlog API application
- The MyBlog Auth application
- PostgreSQL database

To stop the containers:

```sh
docker-compose down
```

## Project Features

- Clean Architecture implementation
- CQRS pattern with MediatR
- JWT based authentication
- PostgreSQL database
- Domain-Driven Design principles

## Architecture

The solution follows Clean Architecture principles:

- Domain
  - Core - Domain entities and business rules
  - Application - Application services and interfaces
- Infrastructure
  - Infrastructure - External concerns and implementations
  - Jwt - Authentication services
- Presentation
  - WebApi - API endpoints and configuration
