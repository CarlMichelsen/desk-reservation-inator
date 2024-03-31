FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base

# Install Firefox and dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    firefox-esr \
    xvfb \
    && rm -rf /var/lib/apt/lists/*

# Set environment variables
ENV Display=:99
ENV MOZ_HEADLESS=1

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["desk-reservation-inator.sln", "./"]

COPY . .

RUN dotnet restore

RUN dotnet test

RUN dotnet build "./App" -c Release --output /app/build

FROM build AS publish

RUN dotnet publish "./App" -c Release --output /app/publish

FROM base AS final

WORKDIR /app

COPY --from=publish /app/publish .

# Start Xvfb
RUN Xvfb :99 -screen 0 1024x768x24 &

ENTRYPOINT ["dotnet", "./App.dll"]