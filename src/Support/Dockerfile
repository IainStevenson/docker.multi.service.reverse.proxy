#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 443

# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY Proxy/certificates/myInfoRootCA.crt /usr/local/share/ca-certificates/myInfoRootCA.crt
RUN update-ca-certificates

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Support/Support.csproj", "Support/"]
RUN dotnet restore "Support/Support.csproj"
COPY . .
WORKDIR "/src/Support"
RUN dotnet build "Support.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Support.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Support.dll"]
