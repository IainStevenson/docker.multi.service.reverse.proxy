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
COPY ["Store/Store.csproj", "Store/"]
COPY ["Logging/Logging.csproj", "Logging/"]
RUN dotnet restore "Store/Store.csproj"
COPY . .
WORKDIR "/src/Store"
RUN dotnet build "Store.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Store.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Store.dll"]