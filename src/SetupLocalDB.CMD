:: These folders are required for MongoDB to persist the data between container builds.
:: they are referenced in the MongoDB container setup as volumes
IF NOT EXIST "%APPDATA%\MongoDB\." MD %APPDATA%\MongoDB
IF NOT EXIST "%APPDATA%\MongoDB\Data\." MD %APPDATA%\MongoDB\Data
IF NOT EXIST "%APPDATA%\MongoDB\Logs\." MD %APPDATA%\MongoDB\Logs
GOTO Done

:Done
@Echo Local MongoDB data folders exist