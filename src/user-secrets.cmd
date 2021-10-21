SETLOCAL
@echo off
::===================================================================================================
:: Execute this command script to setup the Visual Studio User-Secrets required to make this all work
:: If you have setup to use social identities from google, live or github you may optionally use them
:: use null as a literal as placeholders for missing optionals.
::
:: Parameters are required unless stated.
::
::===================================================================================================
:: %~1 action (set|remove)
:: %~2 domain e.g. whatever.tld
:: %~3 database admin user e.g. admin
:: %~4 database admin password e.g. password
:: %~5 app storage user name e.g. datauser
:: %~6 app storage user password e.g. password
:: %~7 Mvc shared Client Id e.g. Mvc
:: %~8 mvc shared client secret e.g. secret
:: %~9  (optional) google client id e.g. id
:: %~10 (optional) google client secret e.g. secret
:: %~11 (optional) microsoft client id e.g. id
:: %~12 (optional) microsoft client secret e.g. secret
:: %~13 (optional) github client id e.g. id
:: %~14 (optional) github client secret e.g. secret
::
:: 
SET USERSECRETS-DOMAIN=%~2
if "%~1"=="set" GOTO setup
if "%~1"=="remove" GOTO remove
if "%~8"=="" GOTO syntax
Goto Syntax

:setup
@ECHO Performing setup
:: MONGO
SET USERSECRETS_Action=%~1
SET USERSECRETS-MONGO_INITDB_DATABASE=admin
SET USERSECRETS-MONGO_INITDB_USERNAME=%~3
SET USERSECRETS-MONGO_INITDB_PASSWORD=%~4
SET USERSECRETS-MONGO_STORAGE_USERNAME=%~5
SET USERSECRETS-MONGO_STORAGE_PASSWORD=%~6
SET USERSECRETS-MONGO-ConnectionString=mongodb://%USERSECRETS-MONGO_STORAGE_USERNAME%:%USERSECRETS-MONGO_STORAGE_PASSWORD%@mongo.%USERSECRETS-DOMAIN%:27017
SET USERSECRETS-STORE-ClientId=%~7
SET USERSECRETS-STORE-ClientSecret=%~8
SET USERSECRETS-SUPPORT-ClientId=%~7
SET USERSECRETS-SUPPORT-ClientSecret=%~8

if "%~9"=="" GOTO execute
if NOT "%~9"=="null" SET USERSECRETS-IDENTITY-Google-ClientId=%~9

SHIFT
if "%~9"=="" GOTO execute
if NOT "%~9"=="null" SET USERSECRETS-IDENTITY-Google-ClientSecret=%~9

SHIFT
if "%~9"=="" GOTO execute
if NOT "%~9"=="null" SET USERSECRETS-IDENTITY-Microsoft-ClientId=%~9

SHIFT
if "%~9"=="" GOTO execute
if NOT "%~9"=="null" SET USERSECRETS-IDENTITY-Microsoft-ClientSecret=%~9

SHIFT
if "%~9"=="" GOTO execute
if NOT "%~9"=="null" SET USERSECRETS-IDENTITY-GitHub-ClientId=%~9

SHIFT
if "%~9"=="" GOTO execute
if NOT "%~9"=="null" SET USERSECRETS-IDENTITY-GitHub-ClientSecret=%~9

GOTO execute


:execute
@ECHO Performing %~1 with...
SET USERSECRETS-
PAUSE

::
:: API
::
pushd api
@ECHO.
@ECHO Processing API
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Identity:Authority https://%USERSECRETS-DOMAIN%/identity
dotnet user-secrets %USERSECRETS_Action% Identity:Audience   https://%USERSECRETS-DOMAIN%/identity/resources
@echo Secrets for %CD%
dotnet user-secrets list
popd

::
:: Identity
::
pushd identity
@ECHO.
@ECHO Processing IDENTITY
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString  %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Service:Domain %USERSECRETS-DOMAIN%

IF DEFINED USERSECRETS-IDENTITY-Google-ClientId dotnet user-secrets SET Google:ClientId %USERSECRETS-IDENTITY-Google-ClientId%
IF NOT DEFINED USERSECRETS-IDENTITY-Google-ClientId dotnet user-secrets REMOVE Google:ClientId %USERSECRETS-IDENTITY-Google-ClientId%
IF DEFINED USERSECRETS-IDENTITY-Google-ClientSecret dotnet user-secrets SET Google:ClientSecret %USERSECRETS-IDENTITY-Google-ClientSecret%
IF NOT DEFINED USERSECRETS-IDENTITY-Google-ClientSecret dotnet user-secrets REMOVE Google:ClientSecret 

IF DEFINED USERSECRETS-IDENTITY-Microsoft-ClientId dotnet user-secrets SET Microsoft:ClientId %USERSECRETS-IDENTITY-Microsoft-ClientId%
IF NOT DEFINED USERSECRETS-IDENTITY-Microsoft-ClientId dotnet user-secrets REMOVE Microsoft:ClientId
IF DEFINED USERSECRETS-IDENTITY-Microsoft-ClientSecret dotnet user-secrets SET Microsoft:ClientSecret %USERSECRETS-IDENTITY-Microsoft-ClientSecret%
IF NOT DEFINED USERSECRETS-IDENTITY-Microsoft-ClientSecret dotnet user-secrets REMOVE Microsoft:ClientSecret

IF DEFINED USERSECRETS-IDENTITY-GitHub-ClientId dotnet user-secrets SET GitHub:ClientId %USERSECRETS-IDENTITY-GitHub-ClientId%
IF DEFINED USERSECRETS-IDENTITY-GitHub-ClientSecret dotnet user-secrets SET GitHub:ClientSecret %USERSECRETS-IDENTITY-GitHub-ClientSecret%
IF NOT DEFINED USERSECRETS-IDENTITY-GitHub-ClientId dotnet user-secrets REMOVE GitHub:ClientId
IF NOT DEFINED USERSECRETS-IDENTITY-GitHub-ClientSecret dotnet user-secrets REMOVE GitHub:ClientSecret
@echo Secrets for %CD%
dotnet user-secrets list
popd
::
:: Store
::
pushd store
@ECHO.
@ECHO Processing STORE
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString  %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientId %USERSECRETS-STORE-ClientId%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientSecret %USERSECRETS-STORE-ClientSecret%
dotnet user-secrets %USERSECRETS_Action% Authentication:Authority https://%USERSECRETS-DOMAIN%/identity
dotnet user-secrets %USERSECRETS_Action% Api:BaseUri https://api.%USERSECRETS-DOMAIN%/api/resources

@echo Secrets for %CD%
dotnet user-secrets list
popd
::
:: Support
::
pushd support
@ECHO.
@ECHO Processing SUPPORT
@ECHO.
dotnet user-secrets %USERSECRETS_Action% Mongo:ConnectionString  %USERSECRETS-MONGO-ConnectionString%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientId %USERSECRETS-SUPPORT-ClientId%
dotnet user-secrets %USERSECRETS_Action% Authentication:ClientSecret %USERSECRETS-SUPPORT-ClientSecret%
dotnet user-secrets %USERSECRETS_Action% Authentication:Authority https://%USERSECRETS-DOMAIN%/identity
dotnet user-secrets %USERSECRETS_Action% Api:BaseUri https://api.%USERSECRETS-DOMAIN%/api/resources
@echo Secrets for %CD%
dotnet user-secrets list
popd
::
:: Finished
::
@ECHO Finished Configuration of User-Secrets
GOTO end

:syntax
@ECHO.
@ECHO This command requires multiple parameters
@ECHO.
@ECHO user-secrets <set|remove> mydomain.org admin admin storage storagepass Mvc secret [googleid googlesecret microsoftid microsoftsecret githubid githubsecret]
@ECHO.
@ECHO All parameters are required except those inside [ ] which are optional and null is permitted as a placeholder
@ECHO.
@ECHO.

:end
ENDLOCAL