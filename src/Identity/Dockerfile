#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

EXPOSE 443

# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Identity/Identity.csproj", "Identity/"]
COPY ["Data.Model.Storage.MongoDB/Data.Model.Storage.MongoDB.csproj", "Data.Model.Storage.MongoDB/"]
COPY ["Pluralizer/Pluralizer.csproj", "Pluralizer/"]
COPY ["Storage/Storage.csproj", "Storage/"]
COPY ["Data.Model.Storage/Data.Model.Storage.csproj", "Data.Model.Storage/"]
COPY ["Data.Model/Data.Model.csproj", "Data.Model/"]
COPY ["Identity.Storage.MongoDB/Identity.Storage.MongoDB.csproj", "Identity.Storage.MongoDB/"]
COPY ["Logging/Logging.csproj", "Logging/"]
RUN dotnet restore "Identity/Identity.csproj"
COPY . .
WORKDIR "/src/Identity"
RUN dotnet build "Identity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Identity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Identity.dll"]