# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Hackathon.HealthMed.Agendar.Service/Hackathon.HealthMed.Agendar.Service.csproj", "Hackathon.HealthMed.Agendar.Service/"]
COPY ["Hackathon.HealthMed.Agendar.Infra/Hackathon.HealthMed.Agendar.Infra.csproj", "Hackathon.HealthMed.Agendar.Infra/"]
COPY ["Hackathon.HealthMed.Agendar.Domain/Hackathon.HealthMed.Agendar.Domain.csproj", "Hackathon.HealthMed.Agendar.Domain/"]
COPY ["Hackathon.HealthMed.Agendar.IoC/Hackathon.HealthMed.Agendar.IoC.csproj", "Hackathon.HealthMed.Agendar.IoC/"]
COPY ["Hackathon.HealthMed.Agendar.Application/Hackathon.HealthMed.Agendar.Application.csproj", "Hackathon.HealthMed.Agendar.Application/"]
RUN dotnet restore "./Hackathon.HealthMed.Agendar.Service/Hackathon.HealthMed.Agendar.Service.csproj"
COPY . .
WORKDIR "/src/Hackathon.HealthMed.Agendar.Service"
RUN dotnet build "./Hackathon.HealthMed.Agendar.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Hackathon.HealthMed.Agendar.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Hackathon.HealthMed.Agendar.Service.dll"]