::
:: Create a host default certificate for ASP.NET (Kestrel) applications signed by the root CA 
:: Add the certificate to the Output folder and add settings to the user secrets for these projects
:: Note the Project secrets id's should be same for whoever runs this solution as they are set in the .csproj files
::
@ECHO off
COLOR o7

setlocal
CALL Gen-Vars.CMD
::
:: Generate the default certificate with the necessary DNS entries
::
:GenHostCert
IF NOT EXIST ..\myRootCA.pfx GOTO RunGenRoot
IF EXIST ..\myHost.pfx	 GOTO ConfigureSecrets
@ECHO -------------------------------------------------------------------------------
@ECHO.
@ECHO These settings will be applied (from domains.ext).
@ECHO Please pay check the 'DNS.x values' as correct.
@ECHO.
TYPE domains.conf :: Provides certificate alt_names's for all non production environments
@ECHO.
@ECHO If these settings are not correct please cancel (Control-C) and edit the 
@ECHO domains.ext file and try again.
@ECHO.
@ECHO Control-C to abort or,
PAUSE
@ECHO.
@ECHO Generating the Self-Signed Host Certificate- * NOT FOR PRODUCTION USE * ...
@ECHO.
openssl req -new -nodes -newkey rsa:2048 -keyout ../myHost.key -out ../myHost.csr -subj "/CN=localhost"
openssl x509 -req -sha256 -days 1024 -in ../myHost.csr -CA ../myRootCA.pem -CAkey ../myRootCA.key -CAcreateserial -extfile domains.conf -out ../myHost.crt
openssl pkcs12 -export -inkey ../myHost.key -in ../myHost.crt -certfile ../myRootCA.crt -out ../myHost.pfx 
@ECHO.
@ECHO Host certificate created.
GOTO ConfigureSecrets
::
:: Configure User Secrets so that Kestrel picks the myHost.pfx as its default and serves all of the DNS via SNI
::
:ConfigureSecrets
@ECHO.
@ECHO Certificates exist, Configuring user secrets...
@ECHO.
::
:: Using specifc user-secrets from this folder to avoid fiddly PUSHD/POPD - the id's dont change themselves after project creation.
::
:: API
::
::dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 remove Kestrel:Certificates:Default:Path
::dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myHost.pfx
dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 set Kestrel:Certificates:Default:Password %PASSWORD%
::
:: Identity
::
::dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 remove Kestrel:Certificates:Default:Path
::dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myHost.pfx
dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 set Kestrel:Certificates:Default:Password %PASSWORD%
::
:: Store
::
::dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 remove Kestrel:Certificates:Default:Path
::dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myHost.pfx
dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 set Kestrel:Certificates:Default:Password %PASSWORD%
::
:: Support
::
::dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 remove Kestrel:Certificates:Default:Path
::dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myHost.pfx
dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 set Kestrel:Certificates:Default:Password %PASSWORD%

@ECHO.
@ECHO ASP.NET Secrets configured.
@ECHO.
::
:: Deliver certificates and other files to the proxy folder for docker builds and the .NET core secrets folder
::
@ECHO COPYing myHost.PFX to user-secrets ASP.NET\https folder
COPY /Y ..\myHost.pfx %APPDATA%\ASP.NET\https\myHost.pfx > nul
IF NOT EXIST %APPDATA%\ASP.NET\https\myHost.pfx GOTO InstallFailed
@ECHO COPYing dhparam.pem to context folder for docker build operations
COPY /Y dhparam.pem ..\ > nul
@ECHO.
@ECHO Certificates delivered to build folders
@ECHO.
::
:: Copy the certificates to support the Graph microservice.
::

::@ECHO COPYing myHost.PFX to graph service SSL folders
IF NOT EXIST ..\graphdb\certificates\bolt\revoked  MD ..\graphdb\certificates\bolt\revoked
IF NOT EXIST ..\graphdb\certificates\https\revoked MD ..\graphdb\certificates\https\revoked
IF NOT EXIST ..\graphdb\certificates\bolt\trusted  MD ..\graphdb\certificates\bolt\trusted
IF NOT EXIST ..\graphdb\certificates\https\trusted MD ..\graphdb\certificates\https\trusted

COPY /Y ..\myHost.key ..\graphdb\certificates\bolt\private.key			> nul
COPY /Y ..\myHost.crt ..\graphdb\certificates\bolt\public.crt			> nul
COPY /Y ..\myHost.crt ..\graphdb\certificates\bolt\trusted\public.crt	> nul
COPY /Y ..\myHost.key ..\graphdb\certificates\https\private.key			> nul
COPY /Y ..\myHost.crt ..\graphdb\certificates\https\public.crt			> nul
COPY /Y ..\myHost.crt ..\graphdb\certificates\https\trusted\public.crt	> nul

:: handled via the docker file copy and ca-update-certificates
::COPY /Y ..\myRootCA.key ..\graphdb\certificates\bolt\trusted\myRootCA.key > nul
::COPY /Y ..\myRootCA.crt ..\graphdb\certificates\bolt\trusted\myRootCA.crt > nul
::COPY /Y ..\myRootCA.key ..\graphdb\certificates\https\trusted\myRootCA.key > nul
::COPY /Y ..\myRootCA.crt ..\graphdb\certificates\https\trusted\myRootCA.crt > nul


::COPY /Y ..\GraphDb\neo4j.conf ..\graphdb\conf\neo4j.conf > nul






IF NOT EXIST ..\graphdb\certificates\https\private.key  GOTO InstallFailed

@ECHO.
@ECHO Certificates delivered to GraphDB build folders
@ECHO.

GOTO Finish

:InstallFailed
@ECHO.
@ECHO -----------------------------------------------------------------------------------------
@ECHO.
@ECHO WARNING: 
@ECHO.
@ECHO The ASP.NET https certificate was not delivered
@ECHO Please fix this by copying the ''..\myHost.pfx'' file to ''%APPDATA%\ASP.NET\https''
@ECHO -----------------------------------------------------------------------------------------
@ECHO.


GOTO Finish

:RunGenRoot
COLOR oc
@ECHO.
@ECHO -----------------------------------------------------------------------------------------
@ECHO WARNING: 
@ECHO The root certificate has not been created.
@ECHO Please run Gen-Root.CMD first!
@ECHO -----------------------------------------------------------------------------------------
@ECHO.

:Finish
CALL ..\SetupLocalDB.cmd
@ECHO.
@ECHO -----------------------------------------------------------------------------------------
@ECHO Job done, Set your startup project to docker-compose and start debugging.
@ECHO -----------------------------------------------------------------------------------------
@ECHO.
endlocal
@echo on