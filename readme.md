# Docker multi container service with reverse proxy

A visual studio oriented orchestrated docker solution for microservice based solutions behind a reverse proxy.

this solution uses docker network to provide container-container dns capabilities.


# Objective

This solution is to provide an orchestrated docker container set of two microservices providing two sections of a ficticious web site.

- Store  - where customers would buy products and services.
- Support - Where customer service and staff support can be provided.

The services will then be fronted by a reverse proxy using NGINX to provide a reverse proxy through to the microservices.

Then external port mapping of the payload microservices will be switched off and prevent direct access to the microservices from outside of the container network.

This solution assumes a ficticious domain name of [mystore.local] which is simulated by spoofing local addressing via the hosts file.

Add this domain to the development environment by adding the following entry to your drivers\etc\hosts file using any suitable text editor run as administator.

```
127.0.0.1 mystore.local
```

# Dependencies

This was developed using:
```
Microsoft Visual Studio Professional 2019 Version 16.7.6
Docker version 20.10.0, build 7287ab3
Postman V7.36.1
```

Postman is set locally to switch off SSL verification but one additional goal is to find a way to switch it back on.

It will be tested with subsequent versions at intervals to try and prevent this repository becoming stale over time. It may work on earlier versions! It may not.

The container orchestration is provided using the Visual Studio docker orchestration support and the docker_compose project is the startup for docker debugging builds and runs.

# Getting started

Download this repository and load it into visual studio.

Set the startup to docker_compose using the right click menu on the solution to set startup project.

Edit your hosts file as described above. Once thta change is saved its active immediately.

Press F5.

If you get build issues perform one or maye more build / clean solution runs and try again before checking anything else.

Depending on your network speed, the first build run may take a while if none of the docker layer dependencies are not already in your docker cache. 

Subsequent re builds will be quicker.

## Containers

Refer to the details in docker-compose project docker-compose.yml and docker-compose.override.yml

The containers are given container names to help avoid conflicts in other developers docker sets (and mine).  WebApp1 is a common image name!

The containers are given host names to provide a container local dns ability, which will be useful for configuring NGINX.

The docker orchestration also provides a named network 'mystore'

Once the project is run the following commands can be used to examine the container network

```
docker network ls
```

Which will display all your configured docker networks. The one you are interested in is the one with the name ending in 'mystore'. e.g. dockercompose12441271731797403570_mystore

```
NETWORK ID     NAME                                        DRIVER    SCOPE
ecf7656d1dc4   bridge                                      bridge    local
f0a6248031f4   dockercompose12441271731797403570_mystore   bridge    local
```

Then you can examine its details using the inspect command using either the name or its id

```
docker network inspect dockercompose12441271731797403570_mystore
docker network inspect f0a6248031f4
```

### Container 1

ASP.NET Core MVC web site simply providing a basic site identifying index page as the store site

```
Container name: store.mystore.local
hostname:		store.mystore.local
```
### Container 2

ASP.NET Core MVC web site simply providing a basic site identifying index page as the support site
```
Container name: support.mystore.local
hostname:		support.mystore.local
```

## Container 3
NGINX - Configured as reverse proxy to serve containers 1 and 2 mapped via the /store and /support path segments
```
Container name: proxy.mystore.local
hostname:		proxy.mystore.local
Ports:			80, 443
```


## Addressing scheme 


```
Default Store access 		https://mystore.local is served direct from container 1 index view
Store access via path		https://mystore.local/store  is served direct from container 1 index view
Support access via path		https://mystore.local/support is served direct from container 2 index view
```

URL inter-site redirects from Container 1 to Container 2 and vis-a-versa work as expected and intra site urls using controller actions work as expected


Example index.cshtml from WebApp1 shows a self referenceing controller action link and a standard domain relative ``` href="/support"``` link to the support site from the store;

```
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome to the Store</h1>
    <p>Learn about <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">our privacy policy</a></p>

    <p>Get some <a class="nav-link text-dark" href="/support">support</a></p>
</div>
```


# Testing

Included in the solution is an evolving postman script to cycle through the available urls and test the sites accessibility via the desired url addressing scheme.

Import mystore.local.postman_collection.json into postman and it will create a collection called mystore.local from which you can run single tests or complete test runs.


# Future changes

- Add an identity server micro service to provide sign on via social logins (google+) and local (staff) logins. 
- Add a mongodb storage service for identity service.
- Differentiate staff and customers through the token claims
- Discriminate access to the staff and customer services.
- Split support into customer service (Customer) and Staff support (Staff) services
- Add a mongodb storage service for product and sales data persistence.

