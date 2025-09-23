FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
ENV TZ="Europe/Berlin"
WORKDIR /App

# Copy everything
COPY . ./

# Restore as distinct layers

RUN dotnet restore
# Build and publish a release
RUN dotnet publish Bohntemps.sln -f net8.0 -c Release --property:PublishDir=out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "Bohntemps.dll"]
