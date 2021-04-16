::
:: Gen-Root.CMD
::
:: Create a root certificate to provide a self signed Trusted root Certificate for development host and microservices
::
@ECHO off
setlocal
CALL Gen-Vars.CMD
@ECHO -------------------------------------------------------------------------------
@ECHO.
@ECHO       DISCLAIMER: These are self-signed certificates and are;
@ECHO.
@ECHO *             NOT FOR USE IN PRODUCTION SYSTEMS                               *
@ECHO.
@ECHO   By using this script you agree to use them only in 
@ECHO   non-production AND non public facing environments
@ECHO.
@ECHO -------------------------------------------------------------------------------
@ECHO.
@ECHO NOTE: The certificate files are ignored in the repository and must be generated.
@ECHO.
@ECHO Control-C to abort or,
PAUSE
::
IF EXIST ../myRootCA.pfx GOTO RootCAExists
::
:GenRootCA
@ECHO -------------------------------------------------------------------------------
@ECHO.
@ECHO You will be required to manually input a pasword, Please use '%PASSWORD%' 
@ECHO to ensure its consistent with the registered secrets file and easy to remember. 
@ECHO.
@ECHO This is NOT a production secret. If you wish to change that go ahead but do it 
@ECHO in the GenVARS.CMD script, delete the current output sub folder and run again.
@ECHO.
@ECHO.
@ECHO Generating the Self-Signed Root CA Certificate...
@ECHO.
openssl req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout ../myRootCA.key -out ../myRootCA.pem -subj "/C=UK/ST=London/L=London/O=Development-Root-CA/OU=Development/CN=Development-Root-CA"
openssl x509 -outform pem -in ../myRootCA.pem -out ../myRootCA.crt
openssl pkcs12 -export -inkey ../myRootCA.key -in ../myRootCA.pem -out ../myRootCA.pfx
@ECHO.
@ECHO.
@ECHO You should Import myRootCA.PFX SPECIFICALLY to the;
@ECHO	LOCAL MACHINE 'Trusted Root Certification Authorities' 
@ECHO certificate store using the password you entered.
@ECHO *** Do not let it choose the store for you. ***
@ECHO.
@ECHO.
@ECHO.
TYPE ImportRootCA.txt
@ECHO.
PAUSE
..\myRootCA.pfx
@ECHO Continue only when the root CA certificate is imported.
PAUSE
GOTO Finish

:RootCAExists
@ECHO The certificate already exists.
GOTO Finish

:Finish
@ECHO.
@ECHO Job done. You may now generate the host certificate using the GEN-HOST command script.
@ECHO.
endlocal
@echo on