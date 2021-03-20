# Purpose

To document the research needed to produce;

* Docker orcestration of a set of microservices
* A common domain URL scheme with different services serving different url paths
* A Secure API service an /api url path
* A common security domain allowing social identities and role based access

The key elements of the configuration is documented in the [issues document](file://issues.md) which lists the isues encountered during development and how they were resolved.

# Dependencies

This solution was developed using:

```
Microsoft Visual Studio Professional 2019 Version 16.7.6
Microsoft Visual Studio Professional 2019 Version 16.8.4
Docker version 20.10.0, build 7287ab3
Docker version 20.10.2, build 2291f61
git version 2.26.2.windows.1
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
```
The container orchestration is provided using the Visual Studio docker orchestration support and the docker_compose project is the startup for solution debugging builds and runs.

# Getting started

- Install git for windows in its default location if you have not done so, or modify src/Proxy/gen-vars.cmd to find the openssl.exe elsewhere
- Download this repository and load it into visual studio.
- to install the domain trusted certificate to your dev host, open a powershell or command window in the ```src/Proxy``` folder and execute ```./gen-root.cmd``` and follow instructions \* 
- to generate default certificates for each microservice execute the ```gen-host.cmd``` and follow instructions.
- Edit your hosts file as described in 'Fictional Domain' below. Once that change is saved it is active immediately.
- Set the startup to docker_compose using the right click menu on the solution to set startup project.
- Press F5.
- If no browser appears start one and navigate to https://mystore.local and you will see the store site.
- Navigate around, when you click Weather Forecast you will need to login, use username: bob Password: bob
- Alternatively or as well, import the mystore.local.postman_collection.json file into postman and run the tests in the mystore.local collection.

\* yes I am an old school command shell scripter. I will convert this, side by side, as powershell later.

Depending on your network speed, the first build run may take a while if none of the docker layer dependencies are not already in your docker cache. 

Subsequent re builds will be quicker.

If you get build issues perform one or more build / clean solution runs and try again before checking anything else. 
If in doubt use docker desktop to remove any failed container builds via the 'cleanup' button

## Fictional Domain

This solution assumes a fictional domain name of ```mystore.local``` which is locally simulated as a real DNS entry by spoofing local addressing via the hosts file.

Add this domain to the development environment by adding the following entry to your ```%SystemRoot%\system32\drivers\etc\hosts``` file using any suitable text editor run as administrator.

```
127.0.0.1 mystore.local
```

Having set that domain name there is a need to generate configure and use self signed certificates to depart from using ```localhost``` as a default.

The overall intention of that is to provide a workable route to configuring different environments (local / test / production) and in parameterised builds and deployments.

Which is now taken care of by the setup steps above and the provided scripts.