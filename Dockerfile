# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["src/DuckBill.Api/DuckBill.Api.csproj", "DuckBill.Api/"]
COPY ["src/DuckBill.Application/DuckBill.Application.csproj", "DuckBill.Application/"]
COPY ["src/DuckBill.Domain/DuckBill.Domain.csproj", "DuckBill.Domain/"]
COPY ["src/DuckBill.Infrastructure/DuckBill.Infrastructure.csproj", "DuckBill.Infrastructure/"]

RUN dotnet restore "DuckBill.Api/DuckBill.Api.csproj"

# Copy all source files
COPY src/ .

# Build and publish
WORKDIR /src/DuckBill.Api
RUN dotnet publish "DuckBill.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy published files
COPY --from=build /app/publish .

# Expose port
EXPOSE 5000

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "DuckBill.Api.dll"]

# Made with Bob
