FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
ENV TZ="Europe/Berlin"
WORKDIR /App

# Copy everything
COPY . ./

# Restore as distinct layers

RUN dotnet restore
# Build and publish a release
RUN dotnet publish Bohntemps/Bohntemps.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:8.0
ENV TZ="Europe/Berlin"
WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "Bohntemps.dll"]
