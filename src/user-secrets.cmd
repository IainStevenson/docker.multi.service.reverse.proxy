SETLOCAL
@echo on
:: DO NOT EXECUTE THIS FILE!!!!!
:: Please create a copy of this file outside Source control (add to .gitignore) 
:: and to that copy ...
:: Please add the configuration values here and run that command.
:: these values will be picked up via the ASP.NET core User Seecrets configuration provider at run time.
:: Also please see the Certificates/GEN-HOST.CMD file for certificate details.
::
:: Parameters
:: %1 action (set|remove)
:: %2 domain
:: %3 admin database name
:: %4 database admin user
:: %5 database admin password
:: %6 app storage user name
:: %7 app storage user password
:: %8 app database name
:: %9 Mvc shared Client Id
:: %10 mvc shared client secret
:: %11 (optional) google client id
:: %12 (optional) google client secret
:: %13 (optional) microsoft client id
:: %14 (optional) microsoft client secret
:: %15 (optional) github client id
:: %16 (optional) github client secret
::
::
SET USERSECRETS-DOMAIN=%2
if "%1"=="set" GOTO setup
if "%1"=="remove" GOTO remove
if "%9"=="" GOTO syntax
Goto Syntax

:setup
@ECHO Performing setup
:: MONGO
SET USERSECRETS_Action=%1
SET USERSECRETS-MONGO_INITDB_DATABASE=%3
SET USERSECRETS-MONGO_INITDB_USERNAME=%4
SET USERSECRETS-MONGO_INITDB_PASSWORD=%5
SET USERSECRETS-MONGO_STORAGE_USERNAME=%6
SET USERSECRETS-MONGO_STORAGE_PASSWORD=%7
SET USERSECRETS-MONGO-ConnectionString=mongodb://%USERSECRETS-MONGO_STORAGE_USERNAME%:%USERSECRETS-MONGO_STORAGE_PASSWORD%@mongo.%USERSECRETS-DOMAIN%:27017
:: API
SET USERSECRETS-DatabaseName=%8
IF NOT "%USERSECRETS-DatabaseName%"=="" SET USERSECRETS-API-DatabaseName=%USERSECRETS-DatabaseName%Api
IF NOT "%USERSECRETS-DatabaseName%"=="" SET USERSECRETS-IDENTITY-DatabaseName=%USERSECRETS-DatabaseName%Identity
IF NOT "%USERSECRETS-DatabaseName%"=="" SET USERSECRETS-STORE-DatabaseName=%USERSECRETS-DatabaseName%Store
IF NOT "%USERSECRETS-DatabaseName%"=="" SET USERSECRETS-SUPPORT-DatabaseName=%USERSECRETS-DatabaseName%Support

SET USERSECRETS-STORE-ClientId=%9
SET USERSECRETS-SUPPORT-ClientId=%9
SHIFT
if "%9"=="" GOTO syntax
SET USERSECRETS-SUPPORT-ClientSecret=%9
SET USERSECRETS-STORE-ClientSecret=%9
SHIFT
if "%9"=="" GOTO syntax
SET USERSECRETS-IDENTITY-Google-ClientId=%9
SHIFT
if "%9"=="" GOTO syntax
SET USERSECRETS-IDENTITY-Google-ClientSecret=%9
SHIFT
if "%9"=="" GOTO syntax
SET USERSECRETS-IDENTITY-Microsoft-ClientId=%9
SHIFT
if "%9"=="" GOTO syntax
SET USERSECRETS-IDENTITY-Microsoft-ClientSecret=%9
SHIFT
if "%9"=="" GOTO syntax
SET USERSECRETS-IDENTITY-GitHub-ClientId=%9
SHIFT
if "%9"=="" GOTO syntax
SET USERSECRETS-IDENTITY-GitHub-ClientSecret=%9
GOTO execute


:execute
@ECHO Performing %1 with...
SET USERSECRETS-
pushd MongoDb
@ECHO.
@ECHO Processing MONGODB
@ECHO.
dotnet user-secrets %USERSECRETS_Action% MONGO_INITDB_USERNAME %USERSECRETS-MONGO_INITDB_USERNAME%
dotnet user-secrets %USERSECRETS_Action% MONGO_INITDB_PASSWORD %USERSECRETS-MONGO_INITDB_PASSWORD%
dotnet user-secrets %USERSECRETS_Action% MONGO_INITDB_DATABASE %USERSECRETS-MONGO_INITDB_DATABASE%
dotnet user-secrets %USERSECRETS_Action% MONGO_STORAGE_USERNAME %USERSECRETS-MONGO_STORAGE_USERNAME%
dotnet user-secrets %USERSECRETS_Action% MONGO_STORAGE_PASSWORD %USERSECRETS-MONGO_STORAGE_PASSWORD%
@echo Secrets for %CD%
dotnet user-secrets list
popd
pushd api
@ECHO.
@ECHO Processing API
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Mongo:DatabaseName %USERSECRETS-API-DatabaseName%
@echo Secrets for %CD%
dotnet user-secrets list
popd
pushd identity
@ECHO.
@ECHO Processing IDENTITY
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString  %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Mongo:DatabaseName %USERSECRETS-IDENTITY-DatabaseName%

IF NOT "%USERSECRETS-IDENTITY-Google-ClientId%"=="" dotnet user-secrets %USERSECRETS_Action% Google:ClientId %USERSECRETS-IDENTITY-Google-ClientId%
IF NOT "%USERSECRETS-IDENTITY-Google-ClientSecret%"=="" dotnet user-secrets %USERSECRETS_Action% Google:ClientSecret %USERSECRETS-IDENTITY-Google-ClientSecret%
IF "%USERSECRETS-IDENTITY-Google-ClientId%"=="" dotnet user-secrets REMOVE Google:ClientId
IF "%USERSECRETS-IDENTITY-Google-ClientSecret%"=="" dotnet user-secrets REMOVE Google:ClientSecret

IF NOT "%USERSECRETS-IDENTITY-Microsoft-ClientId%"=="" dotnet user-secrets %USERSECRETS_Action% Microsoft:ClientId %USERSECRETS-IDENTITY-Microsoft-ClientId%
IF NOT "%USERSECRETS-IDENTITY-Microsoft-ClientSecret%"=="" dotnet user-secrets %USERSECRETS_Action% Microsoft:ClientSecret %USERSECRETS-IDENTITY-Microsoft-ClientSecret%
IF "%USERSECRETS-IDENTITY-Microsoft-ClientId%"=="" dotnet user-secrets REMOVE Microsoft:ClientId
IF "%USERSECRETS-IDENTITY-Microsoft-ClientSecret%"=="" dotnet user-secrets REMOVE Microsoft:ClientSecret

IF NOT "%USERSECRETS-IDENTITY-GitHub-ClientId%"=="" dotnet user-secrets %USERSECRETS_Action% GitHub:ClientId %USERSECRETS-IDENTITY-GitHub-ClientId%
IF NOT "%USERSECRETS-IDENTITY-GitHub-ClientSecret%"=="" dotnet user-secrets %USERSECRETS_Action% GitHub:ClientSecret %USERSECRETS-IDENTITY-GitHub-ClientSecret%
IF "%USERSECRETS-IDENTITY-GitHub-ClientId%"=="" dotnet user-secrets REMOVE GitHub:ClientId
IF "%USERSECRETS-IDENTITY-GitHub-ClientSecret%"=="" dotnet user-secrets REMOVE GitHub:ClientSecret
@echo Secrets for %CD%
dotnet user-secrets list
popd
pushd store
@ECHO.
@ECHO Processing STORE
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString  %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Mongo:DatabaseName %USERSECRETS-STORE-DatabaseName%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientId %USERSECRETS-STORE-ClientId%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientSecret %USERSECRETS-STORE-ClientSecret%
@echo Secrets for %CD%
dotnet user-secrets list
popd
pushd support
@ECHO.
@ECHO Processing SUPPORT
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString  %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Mongo:DatabaseName %USERSECRETS-SUPPORT-DatabaseName%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientId %USERSECRETS-SUPPORT-ClientId%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientSecret %USERSECRETS-SUPPORT-ClientSecret%
@echo Secrets for %CD%
dotnet user-secrets list
popd
GOTO end

:Syntax
Please supply the follwing options

User-Secrets [set|remove]

:syntax
@ECHO.
@ECHO This command requires multiple parameters
@ECHO.
@ECHO user-secrets set local.myinfo.world admin admin admin myInfoUser storagepass myInfo myInfo.Mvc secret googleid googlesecret
@ECHO.

:end
ENDLOCAL