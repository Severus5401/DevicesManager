# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Install NodeJs
RUN apt-get update && \
apt-get install -y wget && \
apt-get install -y gnupg2 && \
wget -qO- https://deb.nodesource.com/setup_16.x | bash - && \
apt-get install -y --fix-missing build-essential nodejs 
# End Install

WORKDIR /app

# Copy csproj and restore as distinct layers
COPY . .
RUN dotnet restore

RUN dotnet publish DevicesManager.Web/DevicesManager.Web.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "DevicesManager.Web.dll"]