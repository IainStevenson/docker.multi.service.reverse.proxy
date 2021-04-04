::
:: Gen-Root.CMS
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
@ECHO Control-C to abort or,
PAUSE
::
IF EXIST certificates/myStoreRootCA.pfx GOTO GenHostCertExists
::
:GenRootCA
@ECHO -------------------------------------------------------------------------------
@ECHO.
@ECHO You may be required to input a pasword, Please use '%PASSWORD%' to ensure its 
@ECHO consistent with the registered secrets file and easy to remember. 
@ECHO This is NOT a production secret. If you wish to change that go ahead but do it 
@ECHO in this script, delete the current certificates and run again.
@ECHO.
@ECHO.
@ECHO Generating the Self-Signed Root CA Certificate...
@ECHO.
openssl req -x509 -nodes -new -sha256 -days 1024 -newkey rsa:2048 -keyout certificates/myStoreRootCA.key -out certificates/myStoreRootCA.pem -subj "/C=UK/ST=London/L=London/O=Development-Root-CA/OU=Development/CN=Development-Root-CA"
openssl x509 -outform pem -in certificates/myStoreRootCA.pem -out certificates/myStoreRootCA.crt
openssl pkcs12 -export -inkey certificates/myStoreRootCA.key -in certificates/myStoreRootCA.pem -out certificates/myStoreRootCA.pfx
@ECHO.
@ECHO.
@ECHO You should Import myStoreRootCA.PFX SPECIFICALLY to the;
@ECHO	LOCAL MACHINE 'Trusted Root Certification Authorities' 
@ECHO certificate store using the password you entered.
@ECHO *** Do not let it choose the store for you. ***
@ECHO.
@ECHO.
@ECHO.
TYPE ImportRootCA.txt
@ECHO.
PAUSE
certificates\myStoreRootCA.pfx
@ECHO Continue only when the root CA certificate is imported.
PAUSE
GOTO Finish
:GenHostCertExists
@ECHO The certificate already exists.
GOTO Finish
:Finish
@ECHO.
@ECHO Job done. You may now generate the host certificate.
@ECHO.
endlocal
@echo on