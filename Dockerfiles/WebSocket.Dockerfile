FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /
COPY ["Src/MyBlog.WebSocket/MyBlog.WebSocket.csproj", "Src/MyBlog.WebSocket/"]
COPY ["Src/MyBlog.Application/MyBlog.Application.csproj", "Src/MyBlog.Application/"]
COPY ["Src/MyBlog.Core/MyBlog.Core.csproj", "Src/MyBlog.Core/"]
COPY ["Src/MyBlog.Postgres/MyBlog.Postgres.csproj", "Src/MyBlog.Postgres/"]
COPY ["Src/MyBlog.RabbitMq/MyBlog.RabbitMq.csproj", "Src/MyBlog.RabbitMq/"]
COPY ["Src/MyBlog.Redis/MyBlog.Redis.csproj", "Src/MyBlog.Redis/"]
COPY ["Src/MyBlog.Jwt/MyBlog.Jwt.csproj", "Src/MyBlog.Jwt/"]
RUN dotnet restore "Src/MyBlog.WebSocket/MyBlog.WebSocket.csproj"
COPY . .
RUN dotnet publish "Src/MyBlog.WebSocket/MyBlog.WebSocket.csproj" -c Release -o /app --no-restore

FROM base AS final
COPY --from=build /app .
ENTRYPOINT ["dotnet", "MyBlog.WebSocket.dll"]