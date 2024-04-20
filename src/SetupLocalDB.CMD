:: These folders are required for MongoDB to persist the data between container builds.
:: they are referenced in the MongoDB container setup as volumes
IF NOT EXIST "%APPDATA%\dmsrp\MongoDB\." MD %APPDATA%\dmsrp\MongoDB
IF NOT EXIST "%APPDATA%\dmsrp\MongoDB\Data\." MD %APPDATA%\dmsrp\MongoDB\Data
IF NOT EXIST "%APPDATA%\dmsrp\MongoDB\Logs\." MD %APPDATA%\dmsrp\MongoDB\Logs
GOTO Done

:Done
@Echo Local MongoDB data folders now exists