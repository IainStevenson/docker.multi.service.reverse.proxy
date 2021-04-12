::
:: Create a host default certificate for ASP.NET (Kestrel) applications signed by the root CA 
:: Add the certificate to the Output folder and add settings to the suer secrets for these projects
:: Note the Project secrets id's should be same for whoever runs this solution as they are set in the .csproj files
::
@ECHO off
setlocal
CALL Gen-Vars.CMD
::
:: Generate the default certificate with the necessary DNS entries
::
:GenHostCert
IF NOT EXIST Output/myRootCA.pfx GOTO RunGenRoot
IF EXIST Output/myInfo.pfx	 GOTO ConfigureSecrets
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
openssl req -new -nodes -newkey rsa:2048 -keyout Output/myInfo.key -out Output/myInfo.csr -subj "/CN=localhost"
openssl x509 -req -sha256 -days 1024 -in Output/myInfo.csr -CA Output/myRootCA.pem -CAkey Output/myRootCA.key -CAcreateserial -extfile domains.conf -out Output/myInfo.crt
openssl pkcs12 -export -inkey Output/myInfo.key -in Output/myInfo.crt -certfile Output/myRootCA.crt -out Output/myInfo.pfx 
@ECHO.
@ECHO Host certificate created.
GOTO ConfigureSecrets
::
:: Configure User Secrets so that Kestrel picks the myInfo.pfx as its default and serves all of the DNS via SNI
::
:ConfigureSecrets
@ECHO.
@ECHO Configuring user secrets...
@ECHO.
::
:: Using specifc user-secrets from this folder to avoid PUSHD/POPD nonesnse - the id's dont change themselves after project creation.
::
:: API
::
dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 remove Kestrel:Certificates:Default:Path
dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myInfo.pfx
dotnet user-secrets --id  5151c092-27d2-4f8a-a445-ea3ae9b6e786 set Kestrel:Certificates:Default:Password %PASSWORD%
::
:: Identity
::
dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 remove Kestrel:Certificates:Default:Path
dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myInfo.pfx
dotnet user-secrets --id  70be3d3e-27d5-4fa3-8870-f933385a83e2 set Kestrel:Certificates:Default:Password %PASSWORD%
::
:: Store
::
dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 remove Kestrel:Certificates:Default:Path
dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myInfo.pfx
dotnet user-secrets --id  74a3b9d9-c004-4d94-b127-1bf998c57245 set Kestrel:Certificates:Default:Password %PASSWORD%
::
:: Support
::
dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 remove Kestrel:Certificates:Default:Path
dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 remove Kestrel:Certificates:Default:Password
dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 set Kestrel:Certificates:Default:Path /root/.aspnet/https/myInfo.pfx
dotnet user-secrets --id  15a33753-9d20-4889-817e-133e9eff1e83 set Kestrel:Certificates:Default:Password %PASSWORD%
@ECHO.
@ECHO Secrets configured.
@ECHO.
::
:: Deliver certificates and other files to the proxy folder for docker builds and the .NET core secrets folder
::
COPY /Y Output\myInfo.pfx %APPDATA%\ASP.NET\https\myInfo.pfx
COPY /Y Output\myInfo.crt ..\Proxy\*
COPY /Y Output\myInfo.key ..\Proxy\*
COPY /Y dhparam.pem ..\Proxy\*
@ECHO.
@ECHO Certificates delivered to build folders
@ECHO.
GOTO Finish

:RunGenRoot
@ECHO.
@ECHO The root certificate has not been created.
@ECHO Please run Gen-Root.CMD first!
@ECHO.

:Finish
@ECHO.
@ECHO Job done, Set your startup project to docker-compose and start debugging.
@ECHO.
endlocal
@echo on