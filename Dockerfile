# --- Стейдж 1: Сборка React (Frontend) ---
FROM node:20-alpine AS frontend-build
WORKDIR /src/client
# Копируем файлы манифеста из папки фронтенда
COPY MyProject.Client/package*.json ./
RUN npm install
# Копируем весь исходный код фронтенда
COPY MyProject.Client/ ./
RUN npm run build

# --- Стейдж 2: Сборка ASP.NET (Backend) ---
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Копируем файл проекта бэкенда
COPY MyProject.Server/MyProject.Server.csproj MyProject.Server/
RUN dotnet restore MyProject.Server/MyProject.Server.csproj

# Копируем все файлы бэкенда
COPY MyProject.Server/ MyProject.Server/
WORKDIR /src/MyProject.Server

# Копируем результат сборки фронтенда в папку wwwroot бэкенда
# (Важно: проверьте, что в коде бэкенда включен app.UseStaticFiles())
COPY --from=frontend-build /src/client/dist ./wwwroot 

RUN dotnet publish MyProject.Server.csproj -c Release -o /app/publish

# --- Стейдж 3: Финальный образ (Runtime) ---
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Настройка переменных среды и портов
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

# Healthcheck для основного приложения
HEALTHCHECK --interval=30s --timeout=5s --start-period=15s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "MyProject.Server.dll"]