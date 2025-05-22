FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Src/MyBlog.WebApi/MyBlog.WebApi.csproj", "Src/MyBlog.WebApi/"]
COPY ["Src/MyBlog.Application/MyBlog.Application.csproj", "Src/MyBlog.Application/"]
COPY ["Src/MyBlog.Core/MyBlog.Core.csproj", "Src/MyBlog.Core/"]
COPY ["Src/MyBlog.Postgres/MyBlog.Postgres.csproj", "Src/MyBlog.Postgres/"]
COPY ["Src/MyBlog.Jwt/MyBlog.Jwt.csproj", "Src/MyBlog.Jwt/"]
RUN dotnet restore "Src/MyBlog.WebApi/MyBlog.WebApi.csproj"
COPY . .
WORKDIR "/src/Src/MyBlog.WebApi"
RUN dotnet build "MyBlog.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyBlog.WebApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyBlog.WebApi.dll"]