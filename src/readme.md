# Docker multi container service with reverse proxy

A visual studio oriented orchestrated docker solution for microservice based solutions behind a reverse proxy with SSL transport encryption and endpoint validation.

# Objective

This solution is to provide an orchestrated docker container set of two microservices providing two sections of a web site, being the following features;

- Store  - where customers would buy products and services.
- Support - Where customer service and staff support can be provided.

The services will then be fronted by a reverse proxy using NGINX to provide a reverse proxy through to the microservices.

Then external port mapping will be switched off and prevent direct access to the microservices from outside of the container network.

This will provide a number of advantages;

- Reduce the ports expossed to consumers.
- Reduce the attack surface to one component.
- Provide a means of upgrading with zero service downtime by managing NGINX configuration.
- Provide a means of adding site feature additions.

This solution assumes a ficticious domain name of [mystore.local] which is simulated by spoofing local addressing via the hosts file.

This must be provided in the development environment by adding the following entry to your drivers\etc\hosts file using any suitable text editor run as administator.

```
127.0.0.1 mystore.local
```

# Dependencies

This was developed using:
```
Microsoft Visual Studio Professional 2019 Version 16.7.6
Docker version 20.10.0, build 7287ab3
```
It will be tested with subsequent versions at intervals to try and orevent this repository becoming stale.

It may work on earlier versions! It may not.

The container orchestration is provided using the Visual Studio docker orchestration support and the docker_compose project is the startup for docker debugging builds and runs.

# Getting started

Download this repository and load it into visual studio.

Set the startup to docker_conpose using the right click menu on the solution to set startup project.

Edit your hosts file as described above. Once thta change is saved its active immediately.

Press F5.

If you get build issues perform one or maye more build / clean solution runs and try again before checking anything else.

The first build run may take a while if none of the docker layer dependencies are not already in your docker cache. 
Subsequent re builds will be quicker.

## Containers

Will each be port mapped for host access direct to container, and eventually after the prodyc is successfully added and configured, sealed off and accessed via proxy only

All containers will expose ports 80 and 443

Refer to the details in docker-compose project docker-compose.yml and docker-compose.override.yml

The containers are container names to help avoid conflicts in other developers docker sets (and mine). 

WebApp1 is a common image name!

The containers are given host names to provide a container local dns ability, which will be useful later on.

The docker orchestration is also provided a named network

### Container 1
ASP.NET Core MVC web site simply providing a basic site identifying index page as the store site
```
Container name: store.mystore.local
hostname:		store.mystore.local
Eventual Ports:	80, 443
Initial  Ports:	4302:80, 44302:443
```
### Container 2

ASP.NET Core MVC web site simply providing a basic site identifying index page as the support site
```
Container name: support.mystore.local
hostname:		support.mystore.local
Eventual Ports:	80, 443
Initial  Ports:	4303:80, 44303:443
```

## Container 3
NGINX - Configured as reverse proxy to serve containers 1 and 2 mapped via the /store and /support route segments
```
Container name: proxy.mystore.local
hostname:		proxy.mystore.local
Ports:			80, 443
```

### A note on the dockerfile for this container.

This dockerfile was developed from information leared in this [Pluralsight course](https://app.pluralsight.com/library/courses/containerizing-software-application-docker) by [Nigel Brown](https://app.pluralsight.com/profile/author/nigel-brown), which I recommend for this subject.


## Addressing scheme 

Initially Https
```
Store access via port		https://mystore.local:44302 is served direct from container 1 index view
Support access via port		https://mystore.local:44303 is served direct from container 2 index view
Store access via uri path	https://mystore.local/store receives connection refused
Support access via uri		https://mystore.local/support receives connection refused
```
Initially Http
```
Store access via port		http://mystore.local:4302 is served direct from container 1 index view
Support access via port		http://mystore.local:4303 is served direct from container 2 index view
Store access via uri path	http://mystore.local/store receives connection refused
Support access via uri		http://mystore.local/support receives connection refused
```
# Testing

Included in the solution is an evolving postman script to cycle through the available urls and test the sites accessibility via the desired url addressing scheme.

Import mystore.local.postman_collection.json into postman and it will create a collection called mystore.local from which you can run single tests or complete test runs.


# Future changes

Te solution will be evovle along these lines, as we go each evolution will be tagged and its commit version published here.

Add certificate support to get a self-signed certificate for the domain in place to serve as a specifc domain from the proxy rather that 'localhost'

[Possible solution](https://jamielinux.com/docs/openssl-certificate-authority/)


Eventually - after adding and configuring proxy servgice
Https
```
Store access via port		https://mystore.local:44302 receives connection refused
Support access via port		https://mystore.local:44303 receives connection refused
Store access via uri path	https://mystore.local/store is served direct from container 1 index view
Support access via uri		https://mystore.local/support is served direct from container 2 index view
```
Http
```
Store access via port		http://mystore.local:4302 receives connection refused
Support access via port		http://mystore.local:4303 receives connection refused
Store access via uri path	http://mystore.local/store is served direct from container 1 index view
Support access via uri		http://mystore.local/support is served direct from container 2 index view
```

Force all access via https: so that external (to the container network, incoming to the proxy)  http calls are re-written to https.

http://* -> https://*

Http
```
Store access via port		http://mystore.local:4302 receives connection refused
Support access via port		http://mystore.local:4303 receives connection refused
Store access via uri path	http://mystore.local/store is re-written as  https://mystore.local/store and served direct from container 1 index view
Support access via uri		http://mystore.local/support is re-written as https://mystore.local/support and served direct from container 2 index view
```


Split support into customer service (Customer) and Staff support (Staff) services
Add an identity server micro service to provide sign on via social logins (google+) and local (staff) logins. 
Differentiate staff and customers through the token claims
Discriminate access to the staff and customer services.
Add a mongodb storage service for identity service.
Add a mondodb storage service for product and sales data persistence.

