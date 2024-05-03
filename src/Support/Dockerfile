#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 443

# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Support/Support.csproj", "Support/"]
COPY ["Logging/Logging.csproj", "Logging/"]
RUN dotnet restore "./Support/Support.csproj"
COPY . .
WORKDIR "/src/Support"
RUN dotnet build "./Support.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Support.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Support.dll"]