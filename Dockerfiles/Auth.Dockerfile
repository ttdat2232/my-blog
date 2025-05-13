FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Src/MyBlog.Application/MyBlog.Application.csproj", "Src/MyBlog.Application/"]
COPY ["Src/MyBlog.Core/MyBlog.Core.csproj", "Src/MyBlog.Core/"]
COPY ["Src/MyBlog.Postgres/MyBlog.Postgres.csproj", "Src/MyBlog.Postgres/"]
COPY ["Src/MyBlog.Auth/MyBlog.Auth.csproj", "Src/MyBlog.Auth/"]
COPY ["Src/MyBlog.Jwt/MyBlog.Jwt.csproj", "Src/MyBlog.Jwt/"]
RUN dotnet restore "Src/MyBlog.Auth/MyBlog.Auth.csproj"
COPY . .
WORKDIR "/src/Src/MyBlog.Auth"
RUN dotnet build "MyBlog.Auth.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MyBlog.Auth.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MyBlog.Auth.dll"]