FROM debian:buster-slim AS base

# Install necessary dependencies
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       wget \
       gnupg \
       apt-transport-https \
       ca-certificates \
       tar \
       chromium \
       libglib2.0-0 \
       libnss3 \
       libx11-6 \
       libx11-xcb1 \
       libxcb1 \
       libxcomposite1 \
       libxcursor1 \
       libxdamage1 \
       libxext6 \
       libxfixes3 \
       libxi6 \
       libxrandr2 \
       libxrender1 \
       libxss1 \
       libxtst6 \
       fonts-liberation \
       libasound2 \
       libatk-bridge2.0-0 \
       libatk1.0-0 \
       libatspi2.0-0 \
       libcups2 \
       libdbus-1-3 \
       libdrm2 \
       libegl1 \
       libgbm1 \
       libgtk-3-0 \
       libpango-1.0-0

# Setup Microsoft package feed
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb \
    && dpkg -i packages-microsoft-prod.deb

# Install ASP.NET Core Runtime 8
RUN apt-get update \
    && apt-get install -y --no-install-recommends dotnet-runtime-8.0 \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean

# Set Chrome options for running in headless mode
ENV CHROME_OPTIONS="--headless --no-sandbox --disable-dev-shm-usage --disable-setuid-sandbox --disable-gpu"

# Run Chrome as root user
USER root

# Set permissions for Chrome executable
RUN chmod +x /usr/bin/chromium

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

ENTRYPOINT ["dotnet", "./App.dll"]