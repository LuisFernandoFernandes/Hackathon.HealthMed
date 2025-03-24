# Base de runtime .NET 9.0
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
ENV ASPNETCORE_ENVIRONMENT=Development

# Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["Hackathon.HealthMed.Api/Hackathon.HealthMed.Api.csproj", "Hackathon.HealthMed.Api/"]
COPY ["Hackathon.HealthMed.Application/Hackathon.HealthMed.Application.csproj", "Hackathon.HealthMed.Application/"]
COPY ["Hackathon.HealthMed.Domain/Hackathon.HealthMed.Domain.csproj", "Hackathon.HealthMed.Domain/"]
COPY ["Hackathon.HealthMed.Infra/Hackathon.HealthMed.Infra.csproj", "Hackathon.HealthMed.Infra/"]
COPY ["Hackathon.HealthMed.IoC/Hackathon.HealthMed.IoC.csproj", "Hackathon.HealthMed.IoC/"]

RUN dotnet restore "Hackathon.HealthMed.Api/Hackathon.HealthMed.Api.csproj"

COPY . .
WORKDIR "/src/Hackathon.HealthMed.Api"
RUN dotnet build "Hackathon.HealthMed.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Hackathon.HealthMed.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hackathon.HealthMed.Api.dll"]
