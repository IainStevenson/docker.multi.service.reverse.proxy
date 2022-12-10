IF "%1"=="" GOTO :End
IF "%1"=="up" GOTO :UP
IF "%1"=="down" GOTO :Down

:UP
devenv docker.multi.service.reverse.proxy.sln /clean
docker-compose up -d --build

GOTO :End

:Down
docker-compose down

GOTO :End



:End