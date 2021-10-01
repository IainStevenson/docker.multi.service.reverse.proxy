# Purpose

To document the research and development needed to produce and configure the following;

A secure docker compose orchestration of a small set of micro services running on Linux containers and featuring;

* A secure Reverse Proxy (NGINX) fronting all services.
* A secure store front-end Web site
* A secure support front-end Web site
* A common security domain and single sign on front-end Web site and service
	* (using Identity Server 4) allowing social identities and role based access
* A secure front-end/back-end API service 
	* WebApi with REST methods
* A secure back-end common persistence server 
	* using MongoDB with a common database and separate collections for each service
	* its not a hard reconfigure to keep separate databases
	* Data at rest is not yet encrypted

# Security

From a security perspective the primary objectives are;

* To have a dedicated authentication and authorisation domain 
* Implement FULL SSL for encryption in motion.
implementation for all services in all environments.  * Implement encryption at rest
* Avoid using localhost as a domain name.

## Opinion
If you are asking WHY then lets just say its long been my opinion, and objective, that you could and therefore should.  This solution is working out the how that can be done in all environments.

# Networking

Its a docker controlled subnet inside your machine, analagous to group of VM's inside a VNET in the cloud with a prublic IP address leading only to the reverse proxy font end.

[network](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/network.md)

# Reducing complexity

Even for such a small collection of services there are quite a number of moving parts involved that need configuring to get this up and running.

Efforts have been made to limit this complexity but still there are currently about 2 dozen variables that need setting to get it up and running. 

Half a dozen of those are to do with MongoDB setup, and another half dozen to do with social login support (if you have them).


Special Note: During development of this solution some local environment only credentials may appear in configuration files and the solution will be hardened later to secure all environment settings in the final V1 release, where the secrets will be entirely ephemeral and unrecorded in any files in the repository.

# Getting started

- Download this repository and load it into visual studio.
- Install git for windows in its default location.
	- If git is anywhere else, or you have openssl somewhere else, or modify ```src/Certificates/gen-vars.cmd``` to specify where to find openssl.exe 
- Set the startup to docker_compose using the right click menu on the solution to set startup project.
- Edit your hosts file as described in 'DNS domain name' below. 
	- Once that change is saved it is active immediately.
- Install the self-signed domain trusted certificates to your development host, open a Powershell or command window in the ```src/Certificates``` folder.
	- Generate and install a root certificate execute ```./gen-root.cmd``` and follow instructions \* 
	- Generate default certificates for each micro service execute the ```gen-host.cmd``` and follow instructions.
- Create the following empty folders to persist MongoDB data across container run-times 
	```
	%APPDATA%\MongoDb\Data
	%APPDATA%\MongoDb\Logs
	```
	You can make this easy by executing the '''src\SetupLocalDB.CMD'''
- After generating the certificates run the user-secrets command script with parameters similar to these but for your own settings;
	- parameters are: 
		- action (SET|REMOVE)
		- domain
		- MONGO admin database
		- MONGO admin user
		- MONGO admin password
		- MONGO connection string username
		- MONGO connection string password
		- Identity shared client id
		- Identity shared client secret
		- [optional] Google login client id
		- [optional] Google login secret
		- [optional] Microsoft login client id
		- [optional] Microsoft login secret
		- [optional] GitHub login client id
		- [optional] GitHub login secret
		
	- ```./user-secrets set local.myinfo.world admin admin storage storagepass Mvc secret googleid googlesecret microsoftid microsoftsecret githubid githubsecret```
	- Note: ATM: if you use different database username and password it needs to also be reflected in ```src/.env``` and ```src/MongoDb/mongo-init.js```
	- if you don't yet have a Google, Microsoft or GitHub external account sign in setup for your app then leave the optional parameters off and the identity server startup will not configure external authentication.
	- In google, Microsoft and GitHub developer consoles you will need to add in your allowed url's as they would normally using the \*.domain They operate by redirect so they are picked up locally on the browser and still work according to the local development machine DNS via the hosts file.	
- Press F5.
	- If no browser appears, start one and navigate to https://local.myInfo.world and you will see the store site.
	- Navigate around, when you click Weather Forecast you will need to login, if logging in locally then use username: bob Password: bob, or use available social logins.
	- Alternatively or as well, import the ```local.myInfo.world.postman_collection.json``` file into postman and run the tests in the ```local.myInfo.world``` collection. This will acquire tokens from Identity Server and check basic MVC and API CRUD functionality.

\* I will convert this, side by side, as Powershell later.
## Patience and Frustration

Depending on your network speed, the first build run may take a while if none of the docker layer dependencies are not already in your docker cache. 

Subsequent re-builds will be quicker.

If you get re-build issues, perform one or more visual studio build / clean solution runs and try again before checking anything else. 

If in doubt use docker desktop to remove any failed container builds via the 'cleanup' button. I noted that sometimes the docker image does not pick up on subtle file changes that are not c# code to force an image rebuild.

## DNS domain name

Add the following domain to the development environment private DNS by adding the following entry to your 
```%SystemRoot%\system32\drivers\etc\hosts``` file using any suitable text editor run as administrator.

```
127.0.0.1 local.myInfo.world
```

NOTE: This is my domain and already registered. Using it in your local development environment or other private environment will work, however should you deploy it to a public service then it wont as it will come to my services which are forked from this starter repository.

You can easily change the domain by globally changing 'myinfo.world' to whatever you like. Note: just changing myInfo will cause problems with certificate names.

Having set that domain name there is a need to generate configure and use self-signed certificates to help in departing from using ```localhost``` as a default domain.

The overall intention of that is to provide a workable secure local environments via self-signed certificates, and use services like (lets encrypt)[https://letsencrypt.org/] to provide other sub domain certificates.

Which is now taken care of by the setup steps above and the provided scripts.



# Dependencies

This solution was developed using:

```
Microsoft Visual Studio Professional 2019 Version 16.7.6
Microsoft Visual Studio Professional 2019 Version 16.8.4
Microsoft Visual Studio Professional 2019 Version 16.9.3
Docker version 20.10.0
Docker version 20.10.2
Docker version 20.10.5
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
Conversely is that this solution will not work properly or at all if those ports are already in use elsewhere.

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

Various markdown files are included focusing on different aspects of the development process;

- [configuration](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/Configuration.md)
- [certificates](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/certificates.md)
- [docker](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/docker.md)
- [http](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/http.md)
- [identity](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/identity.md)
- [linux](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/linux.md)
- [network](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/network.md)
- [solution](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/solution.md)
- [issues document](https://github.com/IainStevenson/docker.multi.service.reverse.proxy/blob/master/src/issues.md)

# Important

When changing domain or product names casing is important

- MongoDB: manage mongo-init.js, delete persisted mongo data : check %APPDATA%/MongoDb/Data
- services : appsettings.*.json
- Proxy: manage Proxy/certificates.domain.conf, gen-host.cmd, user-secrets.cmd

A lot of effort has been expended on reducing the complexity of the configuration and its an ongoing exercise.
