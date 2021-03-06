FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY app/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./app/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine
WORKDIR /app
COPY --from=build-env /app/out .
# ENTRYPOINT ["dotnet", "StoreWeb.dll"]
# heroku uses the following
CMD ASPNETCORE_URLS=http://*:$PORT dotnet StoreWeb.dll