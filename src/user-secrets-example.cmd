SETLOCAL
:: DO NOT EXECUTE THIS FILE!!!!!
:: Please create a copy of this file outside Source control (add to .gitignore) 
:: and to that copy ...
:: Please add the configuration values here and run that command.
:: these values will be picked up via the ASP.NET core User Seecrets configuration provider at run time.
:: Also please see the Certificates/GEN-HOST.CMD file for certificate details.
:: MONGO
SET MONGO_INITDB_DATABASE=
SET MONGO_INITDB_USERNAME
SET MONGO_INITDB_PASSWORD
SET MONGO_STORAGE_USERNAME=
SET MONGO_STORAGE_PASSWORD=
:: API
SET API-DatabaseName=
:: Identity
SET IDENTITY-DatabaseName=
:: Please add Google client credentials for identity server when known
SET IDENTITY-Google-ClientId=
SET IDENTITY-Google-ClientSecret=
:: STORE
SET STORE-DatabaseName=
SET STORE-ClientId=
SET STORE-ClientSecret=
:: SUPPORT
SET SUPPORT-DatabaseName=
SET SUPPORT-ClientId=
SET SUPPORT-ClientSecret=

pushd Mongo

dotnet user-secrets set "MONGO_INITDB_USERNAME" "%MONGO_INITDB_USERNAME%"
dotnet user-secrets set "MONGO_INITDB_PASSWORD" "%MONGO_INITDB_PASSWORD%"
dotnet user-secrets set "MONGO_INITDB_DATABASE" "%MONGO_INITDB_DATABASE%"
dotnet user-secrets set "MONGO_STORAGE_USERNAME" "%MONGO_STORAGE_USERNAME%"
dotnet user-secrets set "MONGO_STORAGE_PASSWORD" "%MONGO_STORAGE_PASSWORD%"
dotnet user-secrets list

popd

pushd api
@echo %CD%
dotnet user-secrets set "Mongo:ConnectionString" "mongodb://%MONGO_STORAGE_USERNAME%:%MONGO_STORAGE_PASSWORD%@mongo.local.myInfo.world:27017"
dotnet user-secrets set "Mongo:DatabaseName" "%API-DatabaseName%"
dotnet user-secrets list
popd
pushd identity

@echo %CD%
dotnet user-secrets set "Mongo:ConnectionString" "mongodb://%MONGO_STORAGE_USERNAME%:%MONGO_STORAGE_PASSWORD%@mongo.local.myInfo.world:27017"
dotnet user-secrets set "Mongo:DatabaseName" "%IDENTITY-DatabaseName%"
dotnet user-secrets set "Google:ClientId" "%IDENTITY-Google-ClientId%"
dotnet user-secrets set "Google:ClientSecret" "%IDENTITY-Google-ClientSecret%"
dotnet user-secrets list
popd
pushd store
@echo %CD%
dotnet user-secrets set "Mongo:ConnectionString" "mongodb://%MONGO_STORAGE_USERNAME%:%MONGO_STORAGE_PASSWORD%@mongo.local.myInfo.world:27017"
dotnet user-secrets set "Mongo:DatabaseName" "%STORE-DatabaseName%"
dotnet user-secrets set "Authentication:ClientId" "%STORE-ClientId%"
dotnet user-secrets set "Authentication:ClientSecret" "%STORE-ClientSecret%"
dotnet user-secrets list
popd
pushd support
@echo %CD%
dotnet user-secrets set "Mongo:ConnectionString" "mongodb://%MONGO_STORAGE_USERNAME%:%MONGO_STORAGE_PASSWORD%@mongo.local.myInfo.world:27017"
dotnet user-secrets set "Mongo:DatabaseName" "myInfoSupport"
dotnet user-secrets set "Mongo:DatabaseName" "%SUPPORT-DatabaseName%"
dotnet user-secrets set "Authentication:ClientId" "%SUPPORT-ClientId%"
dotnet user-secrets set "Authentication:ClientSecret" "%SUPPORT-ClientSecret%"
dotnet user-secrets list
popd
ENDLOCAL