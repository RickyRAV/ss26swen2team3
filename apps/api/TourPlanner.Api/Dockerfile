# ─── Development stage ────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS development
WORKDIR /app
ENV DOTNET_USE_POLLING_FILE_WATCHER=true
ENV ASPNETCORE_URLS=http://+:5000

COPY . .
RUN dotnet restore TourPlanner.Api/TourPlanner.Api.csproj

WORKDIR /app/TourPlanner.Api
CMD ["dotnet", "watch", "run", "--no-restore", "--no-launch-profile"]

# ─── Build stage ──────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY . .
RUN dotnet restore TourPlanner.Api/TourPlanner.Api.csproj
RUN dotnet publish TourPlanner.Api/TourPlanner.Api.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ─── Production stage ─────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS production
WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:5000/api/v1/health || exit 1

ENTRYPOINT ["dotnet", "TourPlanner.Api.dll"]
