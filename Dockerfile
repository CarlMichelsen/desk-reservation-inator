FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base

WORKDIR /app

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

# Install the necessary dependencies for Selenium
RUN apt-get update && apt-get install -y \
    wget \
    unzip \
    libgdiplus \
    libnss3-tools \
    libx11-dev \
    gconf-service \
    libasound2 \
    libatk1.0-0 \
    libcairo2 \
    libcups2 \
    libfontconfig1 \
    libgdk-pixbuf2.0-0 \
    libgtk-3-0 \
    libnspr4 \
    libpango-1.0-0 \
    libxss1 \
    fonts-liberation \
    libappindicator1 \
    libnss3 \
    lsb-release \
    xdg-utils \
    && rm -rf /var/lib/apt/lists/*

RUN wget https://dl.google.com/linux/direct/google-chrome-stable_current_amd64.deb && dpkg -i google-chrome-stable_current_amd64.deb; apt --fix-broken -y install

RUN wget https://chromedriver.storage.googleapis.com/100.0.4896.20/chromedriver_linux64.zip && unzip chromedriver_linux64.zip && mv chromedriver /usr/bin/chromedriver

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "./App.dll"]