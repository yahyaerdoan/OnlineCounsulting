# Build context must be the repo root (project references reach into Modules/*, SharedKernel, etc.)
# Railway service settings: Root Directory = repo root, Dockerfile Path = OnlineConsulting.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore "OnlineConsulting.Api/OnlineConsulting.Api.csproj"
RUN dotnet publish "OnlineConsulting.Api/OnlineConsulting.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Railway injects PORT at runtime; Kestrel must bind to it.
ENTRYPOINT ["sh", "-c", "ASPNETCORE_URLS=http://+:${PORT:-8080} dotnet OnlineConsulting.Api.dll"]
