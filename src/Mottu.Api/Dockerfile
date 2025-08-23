# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Mottu.Api/Mottu.Api.csproj", "src/Mottu.Api/"]
COPY ["src/Mottu.Application/Mottu.Application.csproj", "src/Mottu.Application/"]
COPY ["src/Mottu.Domain/Mottu.Domain.csproj", "src/Mottu.Domain/"]
COPY ["src/Mottu.Infra.Storage/Mottu.Infra.Storage.csproj", "src/Mottu.Infra.Storage/"]
COPY ["src/Mottu.Infra.Utils/Mottu.Infra.Utils.csproj", "src/Mottu.Infra.Utils/"]
COPY ["src/Mottu.Infra.CrossCutting.IoC/Mottu.Infra.CrossCutting.IoC.csproj", "src/Mottu.Infra.CrossCutting.IoC/"]
COPY ["src/Mottu.Infra.Data/Mottu.Infra.Data.csproj", "src/Mottu.Infra.Data/"]
COPY ["src/Mottu.Infra.HostedService/Mottu.Infra.HostedService.csproj", "src/Mottu.Infra.HostedService/"]
RUN dotnet restore "./src/Mottu.Api/Mottu.Api.csproj"
COPY . .
WORKDIR "/src/src/Mottu.Api"
RUN dotnet build "./Mottu.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Mottu.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mottu.Api.dll"]