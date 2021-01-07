# Purpose

Document the detailed technical tasks necessary to set up a docker orchestrated set of back end micro services front ended by a reverse proxy operating over TLS (SSL).

# Dependencies

This was developed using:

```
Microsoft Visual Studio Professional 2019 Version 16.7.6
Docker version 20.10.0, build 7287ab3
Postman V7.36.1 (optional)
```

It will be tested with subsequent versions at future intervals to try and prevent this repository becoming stale over time. It may work on earlier versions! It may not.

The container orchestration is provided using the Visual Studio docker orchestration support and the docker_compose project is the startup for docker debugging builds and runs.

# Getting started

- Download this repository and load it into visual studio.
- Set the startup to docker_compose using the right click menu on the solution to set startup project.
- Edit your hosts file as described in 'Fictional Domain'. Once that change is saved its active immediately.
- Press F5.

If you get build issues perform one or more build / clean solution runs and try again before checking anything else.

Depending on your network speed, the first build run may take a while if none of the docker layer dependencies are not already in your docker cache. 

Subsequent re builds will be quicker.

## Fictional Domain

This solution assumes a fictional domain name of ```mystore.local``` which is locally simulated as a real DNS entry by spoofing local addressing via the hosts file.

Add this domain to the development environment by adding the following entry to your drivers\etc\hosts file using any suitable text editor run as administrator.

```
127.0.0.1 mystore.local
```

Having set that domain name we will need to generate configure and use self signed certificates to depart from using ```localhost``` as a default.

The overall intention of that is to provide environment (local / test / production) alternates of files and configurations to be used in parameterised builds and deployments.

# Overview:

TL;DR - details for refreshing my memory and maybe helping you.

This solution is a windows hosted Visual Studio IDE providing an orchestrated docker solution for micro service based solutions behind a reverse proxy.

This solution uses and explicit docker network to provide container-to-container DNS capabilities, the uses of which will become apparent once container to container network calls are made .Note: this does not refer to browser redirects, but HttpClient calls direct from one container to another inside the docker network subnet. the idea here is not to fix docker network subnet addresses and use them as numbers but to allow them to be different subnets for different developers, controlled by thier docker to avoid subnect collisions, and refer to other servidces by thier local subnet dns names instead.

This solution is to provide an orchestrated docker container set of two micro services providing two sections of a fictitious web site.

- Store  - where customers would buy products and services.
- Support - Where customer service and staff support can be provided.

The services is fronted by NGINX to provide a reverse proxy through to the micro services.

Then external port mapping of the payload micro services are switched off and prevent direct access to the micro services from outside of the container network.


### Start solution and test

Start solution.

In a browser: Navigate to https://mystore.local and you will get an https session at the store landing page after successfully logging in, without any browser complaining.

Run the Postman test collection 

Note: If SSL certificate verification is off (in Postman File/Settings) these will now work. 
If its switched on, which checks the certificate authority chain you must switch on CA Certificates and add the myRootCA.pem in the SSL Certificates tab for it to continue to work.


# Further readin

In the solution src folder there are a number of other markdown files referencing various aspects of the solution such as;

- Self-signed Certificates procedure and notes for integration with the solution
- Docker notes
- Identity and authorisation notes
- Linux notes
- Network notes
- Solution structure notes

# Testing

Included in the solution is an evolving postman script to cycle through the available urls and test the sites accessibility via the desired url addressing scheme.

Import mystore.local.postman_collection.json into postman and it will create a collection called mystore.local from which you can run single tests or complete test runs.

## Notes on Postman testing

Postman is (or should be, via File/Settings) set locally to switch off SSL verification but one additional goal is to find a way to switch it back on.

# Future changes

- IN PROGRESS: Add an identity server micro service to provide sign on via social logins (google+) and local (staff) logins. 
- Add a mongodb storage service for identity service.
- Differentiate staff and customers through the token claims
- Discriminate access to the staff and customer services.
- Split support into customer service (Customer) and Staff support (Staff) services
- Add a mongodb storage service for product and sales data persistence.
