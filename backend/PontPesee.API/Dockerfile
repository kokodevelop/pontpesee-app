# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["PontPesee.API.csproj", "./"]
RUN dotnet restore "PontPesee.API.csproj"

# Copy everything else (excluding files in .dockerignore)
COPY . .

# Build directly to publish (skip separate build step)
# This avoids potential build errors

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
