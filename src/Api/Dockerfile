#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
#EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .
WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# COPY root CA Certificate
COPY --from=build /src/Proxy/myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
# COPY local service certificate
COPY --from=build /src/Proxy/api.mystore.local.crt /etc/ssl/certs/api.mystore.local.crt
COPY --from=build /src/Proxy/api.mystore.local.key /etc/ssl/private/api.mystore.local.key
# Update CA root certificates
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
RUN update-ca-certificates
ENTRYPOINT ["dotnet", "Api.dll"]