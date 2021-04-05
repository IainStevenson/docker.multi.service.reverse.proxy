# Purpose

To document the research and development needed to produce;

A  Docker orchestration of a distinct small set of microservices featuring;
* A store front end site
* A support front end site
* A common security domain front end site (using Identity Server 4) allowing social identities and role based access
* A secure frontend/backend API service 
* A backend common persistence server (MongoDB) with separate databases and collections for each service
* A reverse Proxy (NGINX) fronting all services.

# Local Networking Gotcha

BEWARE: 

The technique used here with a single NGINX reverse proxy listening on localhost(127.0.0.1):443 has 
consequences for other development projects on your development host. 

You will not be able to simultaneously run http/s services from other hosting programs such as IIS Express or self hoested programs in other VS projects due to port 443 and 80 already being bound.

Conversely is that this solution will not work properly or at all if those ports are already in use elesewhere.

## Fictional Domain

This solution now has a real domain name entry of '''myinfo.world''' 

In keeping with usual DNS subdomain conventions the following environment sub-domains will be set up;

| Sub domain         | Use                                    | 
|:-------------------|:---------------------------------------|
| local.myinfo.world | intended for developer isolation       |
| dev.myinfo.world   | intended for developer collaboration   |
| test.myinfo.world  | intended for tester collaboration      |
| demo.myinfo.world  | intended for demonstration             |
| myinfo.world       | for customer use                       |


In each domain the micro-services each use a DNS prefix to those domains within the docker subnet of;

- identity
- api
- store
- support
- mongo

# DNS names

These example DNS host names are for the local top level domain and will be replicated for each of; local, test, demo, world

- identity.myinfo.local
- api.myinfo.local
- store.myinfo.local
- support.myinfo.local
- mongo.myinfo.local

# Setup your environment 

Add these temp domains to the development environment private DNS by adding the following entry to your 
```%SystemRoot%\system32\drivers\etc\hosts``` file using any suitable text editor run as administrator.

```
127.0.0.1 local.myInfo.world
```

Having set that domain name there is a need to generate configure and use self-signed certificates 
to depart from using ```localhost``` as a default.

The overall intention of that is to provide a workable secure local environments via self-signed certificates, 
and use services like (lets encrypt)[https://letsencrypt.org/] to provide other sub domain certificates.



Which is now taken care of by the setup steps above and the provided scripts.


# Getting started

- Install git for windows in its default location.
- If git is anywhere else, or you have openssl somewhere else, or modify '''src/Proxy/gen-vars.cmd''' to locate openssl.exe
- Edit your hosts file as described in 'Fictional Domain' below. Once that change is saved it is active immediately.
- Download this repository and load it into visual studio.
- Install the self-signed domain trusted certificates to your dev host, open a powershell or command window in the ```src/Proxy``` folder and execute ```./gen-root.cmd``` and follow instructions \* 
- To generate default certificates for each microservice execute the ```gen-host.cmd``` and follow instructions.
- Set the startup to docker_compose using the right click menu on the solution to set startup project.
- Press F5.
- If no browser appears, start one and navigate to https://myInfo.local, and optionally  https://myInfo.test, or https://myInfo.demo and you will see the store site.
- Navigate around, when you click Weather Forecast you will need to login, use username: bob Password: bob
- Alternatively or as well, import the myInfo.local.postman_collection.json file into postman and run the tests in the myInfo.local collection.

\* I will convert this, side by side, as powershell later.

Depending on your network speed, the first build run may take a while if none of the docker layer dependencies are not already in your docker cache. 

Subsequent re-builds will be quicker.

If you get build issues perform one or more visual studio build / clean solution runs and try again before checking anything else. 
If in doubt use docker desktop to remove any failed container builds via the 'cleanup' button

# Dependencies

This solution was developed using:

```
Microsoft Visual Studio Professional 2019 Version 16.7.6
Microsoft Visual Studio Professional 2019 Version 16.8.4
Microsoft Visual Studio Professional 2019 Version 16.9.3
Docker version 20.10.0, build 7287ab3
Docker version 20.10.2, build 2291f61
Docker version 20.10.5, build 55c4c88
git version 2.26.2.windows.1
git version 2.30.0.windows.1
```

Optional tools and resources that were used to diagnose and fix problems include;

```
Fildder anywhere
docker desktop
stackoverflow
nginx documentation
docker documentation
Postman V7.36.1
Postman V8.0.3
Postman V8.1.0
```

In the solution, container orchestration is enabled with Linux containers and docker-compose.

The docker_compose project should be your preferred startup for solution debugging builds and runs.


# More Reading:

Various markdown files are included focussing on different aspects of the development process;

- [configuration](file://configuration.md)
- [certificates](file://certificates.md)
- [docker](file://docker.md)
- [http](file://http.md)
- [identity](file://identity.md)
- [linux](file://linux.md)
- [network](file://network.md)
- [solution](file://solution.md)
- [issues document](file://issues.md)
