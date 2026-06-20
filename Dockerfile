# Railway Dockerfile for BlazorCMMS
# This Dockerfile is optimized for Railway deployment

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY ["BlazorApp1/BlazorApp1.csproj", "BlazorApp1/"]
COPY ["BlazorApp1.ServiceDefaults/BlazorApp1.ServiceDefaults.csproj", "BlazorApp1.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "BlazorApp1/BlazorApp1.csproj"

# Copy all source files
COPY . .

# Build the project
WORKDIR "/src/BlazorApp1"
RUN dotnet build "BlazorApp1.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BlazorApp1.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# Copy published files
COPY --from=publish /app/publish .

# Copy startup script
COPY docker-entrypoint.sh /app/docker-entrypoint.sh
RUN chmod +x /app/docker-entrypoint.sh

# Expose ports
EXPOSE 8080

# Run the application with startup script
ENTRYPOINT ["/app/docker-entrypoint.sh"]
