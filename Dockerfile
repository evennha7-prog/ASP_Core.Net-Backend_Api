# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["backend_api.csproj", "./"]
RUN dotnet restore "backend_api.csproj"

# Copy the rest of the application code and publish
COPY . .
RUN dotnet publish "backend_api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render exposes and detects port 8080 or port 10000
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "backend_api.dll"]
