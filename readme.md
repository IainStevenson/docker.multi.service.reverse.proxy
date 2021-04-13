# Purpose

To document the research and development needed to produce the following;

A very secure docker orchestration of a small set of microservices featuring;

* A secure reverse Proxy (NGINX) fronting all services.
* A secure store front-end site
* A secure support front-end site
* A common security domain and single sign on front-end site 
	- (using Identity Server 4) allowing social identities and role based access
* A secure front-end/back-end API service 
* A secure back-end common persistence server 
	- using MongoDB with a common database and separate collections for each service
	- its not a hard reconfigure to keep separate databases
	- Data at rest is not yet encrypted

Some of the primary objectives here are to have a FULL SSL implementation for all services in all environments and avoid using localhost as a domain name.

If you are asking WHY then lets just say its long been my opinion and objective that you could and therefore should.

Note: During development of this solution some local only credentials may appear in config files and the solution will be hardened later to secure all environment settings in the final V1 release

# Getting started

- Install git for windows in its default location.
	- If git is anywhere else, or you have openssl somewhere else, or modify ```src/Certificates/gen-vars.cmd``` to locate openssl.exe
- Download this repository and load it into visual studio.
- Set the startup to docker_compose using the right click menu on the solution to set startup project.
- Edit your hosts file as described in 'DNS domain name' below. 
	- Once that change is saved it is active immediately.
- Install the self-signed domain trusted certificates to your dev host, open a powershell or command window in the ```src/Certificates``` folder.
	- To generate and install a root certificate execute ```./gen-root.cmd``` and follow instructions \* 
	- To generate default certificates for each microservice execute the ```gen-host.cmd``` and follow instructions.
- Create the following empty folders to persist mongo data across container run-times 
	```
	%APPDATA%\MongoDb\Data
	%APPDATA%\MongoDb\Logs
	```
- After generating the certificates run the user-secrets command script with parameters similar to these but for your own settings;
	- parameters are: 
		- action
		- domain
		- MONGO admin database
		- MONGO admin user
		- MONGO admin password
		- MONGO connection string username
		- MONGO connection string password
		- Identity shared client id
		- Identity shared cliet secret
		- [optiona] google login client id
		- [optiona] google login secret
		- [optiona] microsoft login client id
		- [optiona] microsoft login secret
		
	- ```./user-secrets set local.myinfo.world admin admin admin myInfoUser storagepass myInfo myInfo.Mvc secret googleid googlesecret```
	- Note: ATM: if you use different database username and password it needs to also be reflectd in ```docker-compose.override.yml```
	- if you dont yet have a google external setup for your app then leave the last two paramters off and the identity server startup will not cofigure google authentication.
	- In google console you will need to add in your allowed url's as they would normally. they operate by redirect so they are picked up locally on the browser and still work according to the local development machine DNS via the hosts file.	
- Press F5.
	- If no browser appears, start one and navigate to https://local.myInfo.world and you will see the store site.
	- Navigate around, when you click Weather Forecast you will need to login, use username: bob Password: bob
	- Alternatively or as well, import the ```local.myInfo.world.postman_collection.json``` file into postman and run the tests in the ```local.myInfo.world``` collection.

\* I will convert this, side by side, as powershell later.

Depending on your network speed, the first build run may take a while if none of the docker layer dependencies are not already in your docker cache. 

Subsequent re-builds will be quicker.

If you get build issues perform one or more visual studio build / clean solution runs and try again before checking anything else. 
If in doubt use docker desktop to remove any failed container builds via the 'cleanup' button

## DNS domain name

Add the following domain to the development environment private DNS by adding the following entry to your 
```%SystemRoot%\system32\drivers\etc\hosts``` file using any suitable text editor run as administrator.

```
127.0.0.1 local.myInfo.world
```

NOTE: This is my domain and already registered. Using it in your local dev environment or other private environment will work, however should you deploy it to a public service then it wont as it will come to my services which are forked from this starter repo.

Having set that domain name there is a need to generate configure and use self-signed certificates to help in departing from using ```localhost``` as a default domain.

The overall intention of that is to provide a workable secure local environments via self-signed certificates, and use services like (lets encrypt)[https://letsencrypt.org/] to provide other sub domain certificates.

Which is now taken care of by the setup steps above and the provided scripts.



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


# Local Networking Gotcha

BEWARE: 

The technique used here with a single NGINX reverse proxy listening on localhost(127.0.0.1) port 443 has 
consequences for other development projects on your development host.  You will not be able to simultaneously run http/s services from other hosting programs such as IIS Express or 
self hosted programs in other VS projects due to port 443 and 80 already being bound/in use. 
Conversely is that this solution will not work properly or at all if those ports are already in use elesewhere.

## DNS  Domains

This solution has a real domain name entry of ```myinfo.world```

In keeping with usual DNS subdomain conventions the following environment sub-domains will be set up as follows;

| Sub domain         | Use                                    | 
|--------------------|---------------------------------------|
| local.myinfo.world | intended for developer isolation       |
| dev.myinfo.world   | intended for developer collaboration   |
| test.myinfo.world  | intended for tester collaboration      |
| demo.myinfo.world  | intended for demonstration             |
| myinfo.world       | for customer use                       |

In each sub-domain the micro-services each use a DNS prefix of;

- identity
- api
- store
- support
- mongo

These example DNS host names are for the local top level domain and will be replicated for each of; local, test, demo

- identity.local.myinfo.world
- api.local.myinfo.world
- store.local.myinfo.world
- support.local.myinfo.world
- mongo.local.myinfo.world


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

# Important

When changing domain or product names casing is important

- MongoDB: manage mongo-init.js, delete persisted mongo data : check %APPDATA%/MongoDb/Data
- services : appsettings.*.json
- Proxy: manage Proxy/certificates.domain.conf, gen-host.cmd, user-secrets.cmd

A lot of effort has been expended on rducing the complexity of the configuration and its an ongoing exercaise.