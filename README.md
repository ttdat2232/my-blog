# MyBlog

A modern blog application built with .NET 9, implementing clean architecture principles.

## Project Structure

- **Domain**
  - **MyBlog.Core** - Contains domain entities, interfaces, and business logic
  - **MyBlog.Application** - Houses application services, commands, queries using MediatR
- **Infrastructure**
  - **MyBlog.Postgres** - Implements data access using Entity Framework Core
  - **MyBlog.Jwt** - JWT authentication/authorization components
  - **MyBlog.Redis** - Distributed caching implementation
  - **MyBlog.RabbitMq** - Message broker implementation for async communication
- **Presentation**
  - **MyBlog.WebApi** - Primary API endpoints and configuration
  - **MyBlog.Auth** - Authentication service
  - **MyBlog.BackgroundServices** - Background job processing and scheduled tasks

## Technologies

- .NET 9.0
- Entity Framework Core with PostgreSQL
- Redis for distributed caching
- RabbitMQ for message queuing
- MediatR for CQRS pattern
- FluentValidation
- JWT Authentication
- Carter for minimal APIs
- Docker containerization

## Prerequisites

- .NET 9.0 SDK
- Docker and Docker Compose
- Visual Studio 2022 or VS Code (for development)

## Getting Started

### Running Locally

1. Clone the repository

```sh
git clone https://github.com/ttdat2232/my-blog.git
cd ./my-blog
```

2. Configure the following in respective `appsettings.json`:

   - Database connection string
   - Redis connection
   - RabbitMQ settings
   - JWT configuration

3. Run database migrations

```sh
dotnet ef database update -p ./Src/MyBlog.Postgres -s ./Src/MyBlog.WebApi
```

4. Start required services (if running without Docker)

   - PostgreSQL
   - Redis
   - RabbitMQ

5. Run the applications:

```sh
# Start the Auth service
dotnet run --project ./Src/MyBlog.Auth

# Start the Web API
dotnet run --project ./Src/MyBlog.WebApi

# Start background services
dotnet run --project ./Src/MyBlog.BackgroundServices
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

- MyBlog API application
- MyBlog Auth service
- MyBlog Background Services
- PostgreSQL database
- Redis cache server
- RabbitMQ message broker

The services will be available at:

- Web API: http://localhost:5000
- Auth Service: http://localhost:5001
- Background Services: http://localhost:5003
- RabbitMQ Management: http://localhost:15672
- PostgreSQL: localhost:5432
- Redis: localhost:6379

To stop the containers:

```sh
docker-compose down
```

## Project Features

- Clean Architecture implementation
- CQRS pattern with MediatR
- Distributed caching with Redis
- Message queuing with RabbitMQ
- JWT based authentication
- PostgreSQL database
- Domain-Driven Design principles
- Background job processing
- Docker containerization

## Architecture

The solution follows Clean Architecture principles with distributed system patterns:

- **Domain Layer**
  - Core - Domain entities, interfaces, and business rules
  - Application - Application services, CQRS commands/queries
- **Infrastructure Layer**
  - Postgres - Database access and migrations
  - Redis - Distributed caching implementation
  - RabbitMQ - Message broker for async operations
  - JWT - Authentication/authorization services
- **Presentation Layer**
  - WebApi - Primary API endpoints
  - Auth - Authentication service
  - BackgroundServices - Background processing

## Infrastructure Components

- **PostgreSQL**

  - Primary data store
  - Entity Framework Core for ORM
  - Code-first migrations

- **Redis**

  - Distributed caching
  - Session state management
  - Cache invalidation patterns

- **RabbitMQ**

  - Asynchronous message processing
  - Event-driven architecture
  - Dead letter queues
  - Message retry policies

- **Docker**
  - Containerized services
  - Docker Compose orchestration
  - Development and production configurations
