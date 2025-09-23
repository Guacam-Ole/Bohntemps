# Use the official .NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

# Use the .NET 8.0 SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY Bohntemps.sln .
COPY Bohntemps/Bohntemps.csproj Bohntemps/
COPY BeansApi/BohnTemps.BeansApi.csproj BeansApi/
COPY Mastodon/BohnTemps.Mastodon.csproj Mastodon/

# Restore dependencies
RUN dotnet restore "Bohntemps/Bohntemps.csproj"

# Copy the source code
COPY . .

# Publish the application
FROM build AS publish
RUN dotnet publish "Bohntemps/Bohntemps.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Copy the published application
COPY --from=publish /app/publish .

# Set the entry point
ENTRYPOINT ["dotnet", "Bohntemps.dll"]