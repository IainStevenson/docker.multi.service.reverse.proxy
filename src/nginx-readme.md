## NGINX Container build notes


## Dockerfile

The ```Dockerfile``` (case sensitive) is set to expose the required volumes using in the compose, ensure the service never stops unless killed,  and exposes ports for HTTP and HTTPS.

It also copies into place the ```default.conf``` which maps the reverse proxy settings.

## docker-compose.yml

```hostname``` makes each container deterministically DNS addressable from its siblings inc NGINX.
```container_name``` ensures collisions with other projects unlikely.
```image``` ensures collisions with other projects unlikely.
```networks``` binds the container to the same network as its siblings and deterministically sets up a network name and  ensures collisions with other projects unlikely.

## docker-compose-override.yml

Sets environment variables and exposed ports and volumes for the containers.

## default.conf

Initially sets to listen on port 80 and 443.

### Domain specification

```
server_name  localhost mystore.local;
```

Sets a server name for thie NGINX proxy as either ```localhost``` or ```mystore.local```.

Means that in the following URL table ```localhost``` can be replaced with ```mystore.local``` but requires adjustment to ```\Windows\system32\drivers\etc\hosts``` file to direct to 127.0.0.1


### location

With 3 defined location mappings of ```\```, ```\store``` and ```\support\```

The following URL's are supported correctly. Also the self referencing links inside the views of the micro services behave properly.

```
Request URLS 							Routes internall to
https://localhost	 					http://store.mystore.local/store/
https://localhost/store 				http://store.mystore.local/store/
https://localhost/store/home 			http://store.mystore.local/store/home
https://localhost/store/home/privacy 	http://store.mystore.local/store/home/privacy
https://localhost/support 				http://support.mysupport.local/support/
https://localhost/support/home 			http://support.mysupport.local/support/home
https://localhost/support/home/privacy 	http://support.mysupport.local/support/home/privacy
```

With reference to  ```location  /store/ {```  the trailing "\\" is important as it means the application path of ```\store``` is mapped along with all of its sub URL's.

# ASP.NET Core

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

## Forwarded headers

The headers forwarded by the NGINX proxy are set up in the startup. e.g.

```
	app.UseForwardedHeaders(new ForwardedHeadersOptions
	{
		ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
	});
```
