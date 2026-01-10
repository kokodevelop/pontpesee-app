# Dockerfile pour Render.com
# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["PONT_WEBAPP/backend/PontPesee.API/PontPesee.API.csproj", "PontPesee.API/"]
RUN dotnet restore "PontPesee.API/PontPesee.API.csproj"

# Copy everything else and build
COPY PONT_WEBAPP/backend/PontPesee.API/ PontPesee.API/
WORKDIR /src/PontPesee.API
RUN dotnet build "PontPesee.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "PontPesee.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create directories for logs and generated files
RUN mkdir -p /app/logs /app/generated && \
    chmod 777 /app/logs /app/generated

# Copy published files
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "PontPesee.API.dll"]
