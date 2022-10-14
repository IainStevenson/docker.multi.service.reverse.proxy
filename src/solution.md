# Solution organisation notes

Memory refresh on how this solution is organised.

Some of this data in examples is out of date. Beware: Check the actual values in the code!

## Solution structure
The solutions main file set is as follows;
```
src
|_Certificates
|_Api
|_Identity
|_Logging
|-MongoDB
|_Proxy
|_Store
|_Support
docker.multi.service.reverse.proxy.sln
docker-compose.dcproj
docker-compose.yml
docker-compose.override.yml
readme.md
myInfo.local.postman_collection.json
{various}.md
```

The solution has a virtual folder layout as follows;

```
Certificates
Deployment
Infrastructure
Services
Solution Items
Testing
```

### Certificates
Includes command scripts to generate a self-signed root and host certificate to enable SSL.

### Infrastructure
Provides Various libraries will be available for cross cutting concerns.

#### Logging
Provides Request/Response logging middle-ware to aid in request tracing and figuring out any routing problems.
### Services
As specified in the docker-compose file; There are a number of discrete containers build to assemble a set of micro-services.
#### API
Provides a basic but secured API feature
#### Identity
Provides a Custom IdentityServer4 instance to serve as the authorisation domain.
#### Proxy
Provides an NGINX reverse proxy that front ends all other services.
#### Store
Provides the primary commercial landing area.
#### Support
Provides the commercial support landing area.


# Container notes

For current details ples refer to the details in docker-compose project docker-compose.yml and docker-compose.override.yml

The containers are given container names to help avoid conflicts in other developers docker sets (and mine).  

The containers are given host names to provide a container local dns ability, which will be useful for configuring NGINX.

The docker orchestration also provides a named network 'myInfo'

Once the project is run the following commands can be used to examine the container network

```
docker network ls
```

Which will display all your configured docker networks. The one you are interested in is the one with the name ending in 'myInfo'. e.g. dockercompose12441271731797403570_myInfo

```
NETWORK ID     NAME                                        DRIVER    SCOPE
ecf7656d1dc4   bridge                                      bridge    local
f0a6248031f4   dockercompose12441271731797403570_myInfo   bridge    local
```

Then you can examine its details using the inspect command using either the name or its id

```
docker network inspect dockercompose12441271731797403570_myInfo
docker network inspect f0a6248031f4
```


## ASP.NET Core

Applies to Container 1 and Container 2, Api and Identity

## Path mapping

In the ASP.NET Core web applications the application start ups are mapped to their appropriate application path in the startup method,


```
// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
	app.Map("/store", (app) =>
	{ 
		// startup code goes here
	}
```
Substitude /store for /Support in Container 2

## Forwarded headers

The headers forwarded by the NGINX proxy are set up in the startup. e.g.

```
	app.UseForwardedHeaders(new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
	});
```

## Container 1

ASP.NET Core MVC web site simply providing a basic site identifying index page as the store site

```
Container name: store.myInfo.local
hostname:		store.myInfo.local
```
## Container 2

ASP.NET Core MVC web site simply providing a basic site identifying index page as the support site
```
Container name: support.myInfo.local
hostname:		support.myInfo.local
```

## Container 3
NGINX - Configured as reverse proxy to serve containers 1 and 2 mapped via the /store and /support path segments
```
Container name: proxy.myInfo.local
hostname:		proxy.myInfo.local
Ports:			80, 443
```
### NGINX Container build notes

#### Dockerfile

The ```Dockerfile``` (case sensitive) is set to expose the required volumes using in the compose, ensure the service never stops unless killed,  and exposes ports for HTTP and HTTPS.

It also copies into place the ```default.conf``` which maps the reverse proxy settings.

#### docker-compose.yml

```hostname``` makes each container deterministically DNS addressable from its siblings inc NGINX.
```container_name``` ensures collisions with other projects unlikely.
```image``` ensures collisions with other projects unlikely.
```networks``` binds the container to the same network as its siblings and deterministically sets up a network name and  ensures collisions with other projects unlikely.

#### docker-compose-override.yml

Sets environment variables and exposed ports and volumes for the containers.

#### default.conf

Initially sets to listen on port 80 and 443.

#### Domain specification

```
server_name  localhost myInfo.local;
```

Sets a server name for thie NGINX proxy as either ```localhost``` or ```myInfo.local```.

Means that in the following URL table ```localhost``` can be replaced with ```myInfo.local``` but requires adjustment to ```\Windows\system32\drivers\etc\hosts``` file to direct to 127.0.0.1

