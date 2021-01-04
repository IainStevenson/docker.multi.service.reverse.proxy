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


## Notes on Postman testing

Postman is (or should be, via File/Settings) set locally to switch off SSL verification but one additional goal is to find a way to switch it back on.


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

The services will then be fronted by a reverse proxy using NGINX to provide a reverse proxy through to the micro services.

Then external port mapping of the payload micro services will be switched off and prevent direct access to the micro services from outside of the container network.

The main tasks needed are;

- create the vanilla back end services
- create a proxy NGIX front end service
- setup the compose file to provide the necessary containers and network details
- isolate the back end services from direct access from the host network
- create self-signed certificates and root CA.
- configure NGINX to use and redirect to HTTPS to secure transmission from host network to NGINX Proxy.
- expand on that base

## Solution structure
The solutions main file set is as follows;
```
src
|_Logging
|_Proxy
|_WebApp1
|_WebApp2
docker.multi.service.reverse.proxy.sln
docker-compose.dcproj
docker-compose.yml
docker-compose.override.yml
readme.md
mystore.local.postman_collection.json
```

The solution has a virtual folder layout as follows;

```
Infrastructure
Services
Solution Items
docker-compose
```

### Infrastructure
Provides Various libraries will be available for cross cutting concerns.
#### Logging
Provides Request/Response logging middle-ware to aid in request tracing and figuring out any routing problems.
### Services
As specified in the docker-compose file; There are a number of discrete containers build to assemble a set of micro-services.
#### Proxy
Provides an NGINX reverse proxy that front ends all other services.
#### Store
Provides the primary commercial landing area.
#### Support
Provides the commercial support landing area.



# Network topology
```
.--------------------------------------------------------------------------.
!                    .---------------------------------------------------. |
| Development host   | Docker network                                    | |
| Network            |                                                   | |
!                    !                .-----------------.                ! |
|                 ---|--any port---x  | Store   Service |                ! |
|                    |                `-----------------'                ! |
|                    |                    ^                              | |
|                    |                .--------.    .---------------.    ! |
|                 ---|-http/https---->| proxy  | >  | ?     Service |    ! |
| Visual Studio      |                `--------'    `---------------'    ! |
! Browser            !                    v                              ! |
! Postman            !                .-----------------.                ! |
|                 ---|--any port---x  | Support Service |                ! |
|                    |                `-----------------'                ! |
!                    .---------------------------------------------------' !
`--------------------------------------------------------------------------'
```
# Self-Signing certificates for development use.

Swithing to TLS for the NGINX proxy in local/development requires self signed certificates or deep pockets for no real benefit. This is an excercise in how, not why, this can be done. The following proecedure was developed to create and use self-signed SSL certificates in NGINX.

This certificate configuration was taken and adapted to the current solutions build context, from [Here](https://www.digitalocean.com/community/tutorials/how-to-create-a-self-signed-ssl-certificate-for-nginx-in-ubuntu-16-04) and [Here](https://www.obungi.com/2019/05/08/how-to-create-and-use-self-singed-certificates-in-an-ubuntu-docker-container-to-trust-external-resources/) to work within a windows hosted Visual Studio multi-container docker-compose solution with multiple microservices fronted by NGINX.

there were many false starts developing this because the SAN settings detailed are quite particular when getting a clean certificate chain and host identification.

Rather than find OpenSSL for windows and install it locally, this procedure leverages the vanilla tools available as a Windows based VS developer to generate self signed certificates on the linux container, retrieve and store the certificate and configuration files on the windows development host for subsequent builds of those container images.

## Step 1: Create the SSL Certificate

### Access console and prepare container

Start the project using docker-compose as the startup project.

Then in the menu if its not already running; View\Other Windows\Containers

Select proxy.mystore.local
Use the menu icons to start a console.

Then enter the follwing and subsequent commands.

```
apt update
apt install nano
```

## Create the common development root certificate

```
openssl genrsa -out /etc/ssl/private/myRootCA.key 4096
openssl req -x509 -new -nodes -key /etc/ssl/private/myRootCA.key -days 3650 -out /etc/ssl/certs/myRootCA.pem
```
This requires some manual input;
```
UK
London
London
myRootCA
development
myRootCA.development
admin@myRootCA.development
```

NOTE: These can pretty much be anything you like.

#### Convert the root Certificate to PFX to be able to import it into Windows

```
openssl pkcs12 -export -inkey /etc/ssl/private/myRootCA.key -in /etc/ssl/certs/myRootCA.pem -out /etc/ssl/certs/myRootCA.pfx
```

Again, this requires manual input;

```
Enter Export Password: password
Verifying - Enter Export Password: password
```

Any password will do as you are not going to use this for production, are you?

That completes the creation of the self-signed root certificate. 


This can be used as a parent certificate to create many self signed certificates for other projects. 
It will, when installed on the local windows host machine under the 'Local Computer\Trusted Root Certification Authorities\Certificates' folder, automatically enable trust of those child certificates on your development machine, meaning the browser should not pose any certificate issues, nor should there be problems in other tools like postman.


#### Now create the service certificate
##### Create the key
```
openssl genrsa -out /etc/ssl/private/mystore.local.key 2048
```
##### Create a config file
Create a config file : /etc/ssl/certs/mystore.local.conf
Containing;

```
touch /etc/ssl/certs/mystore.local.conf
nano /etc/ssl/certs/mystore.local.conf
```

Paste in the following then Control-x and save the file.

```
[req]
distinguished_name = mystore.local
req_extensions = v3_req
prompt = no
[mystore.local]
C = UK
ST = London
L = London
O = mystore
OU = local
CN = mystore.local
[v3_req]
keyUsage = keyEncipherment, dataEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names
[alt_names]
DNS.1 = mystore.local
DNS.2 = *.mystore.local
```

The critical (specific) elements here are; ```distinguished_name = mystore.local``` ```CN = mystore.local``` and;
```
[alt_names]
DNS.1 = mystore.local
DNS.2 = *.mystore.local
```

In which there is a dependency in the signing section below.

##### Create a certificate signing request
```
openssl req -new -out /etc/ssl/certs/mystore.local.csr -key /etc/ssl/private/mystore.local.key -config /etc/ssl/certs/mystore.local.conf
```

##### Sign the certificate
```
openssl x509 -req -days 365 -CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key -CAcreateserial -extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:mystore.local,DNS:*.mystore.local")) -in /etc/ssl/certs/mystore.local.csr -out /etc/ssl/certs/mystore.local.crt
```

This signing command hooks it up to the previously created self-signed root CA via the ```-CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key```

NOTE: When using printf the back tick  “ “ disables the \n make sure its " "

The '-extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:mystore.local,DNS:*.mystore.local"))' part ensures that the subjectAlterativeNames are being kept in the singed certificate. Otherwhise you will get a irreversable error in Google Chrome.

#### Create a strong Diffie-Hellman group

Which is used in negotiating Perfect Forward Secrecy with clients

This isn't necessary to make TLS work in this context but when you dive into it its a good thing to know about and do anyway.

```
openssl dhparam -out /etc/ssl/certs/dhparam.pem 2048
```

## Step 2: Configure Nginx to Use SSL


We will make a few adjustments to our proxy project for the NGINX configuration.

- We will create a configuration snippet containing our SSL key and certificate file locations.
- We will create a configuration snippet containing strong SSL settings that can be used with any certificates in the future.
- We will adjust our Nginx server blocks to handle SSL requests and use the two snippets above.

### Create a Configuration Snippet Pointing to the SSL Key and Certificate

(NOTE: Modified from source resource due to differing folder structure in NGINX)

```
nano /etc/nginx/conf.d/self-signed.conf
```

Enter the following and save the file.

```
ssl_certificate /etc/ssl/certs/mystore.local.crt;
ssl_certificate_key /etc/ssl/private/mystore.local.key;
```

### Create a Configuration Snippet with Strong Encryption Settings

```
nano /etc/nginx/conf.d/ssl-params.conf
```

Paste in the following and save the file, Control-X Y and Enter

```
# from https://cipherli.st/
# and https://raymii.org/s/tutorials/Strong_SSL_Security_On_nginx.html

ssl_protocols TLSv1 TLSv1.1 TLSv1.2;
ssl_prefer_server_ciphers on;
ssl_ciphers "EECDH+AESGCM:EDH+AESGCM:AES256+EECDH:AES256+EDH";
ssl_ecdh_curve secp384r1;
ssl_session_cache shared:SSL:10m;
ssl_session_tickets off;
ssl_stapling on;
ssl_stapling_verify on;
resolver 8.8.8.8 8.8.4.4 valid=300s;
resolver_timeout 5s;
# Disable preloading HSTS for now.  You can use the commented out header line that includes
# the "preload" directive if you understand the implications.
#add_header Strict-Transport-Security "max-age=63072000; includeSubdomains; preload";
add_header Strict-Transport-Security "max-age=63072000; includeSubdomains";
add_header X-Frame-Options DENY;
add_header X-Content-Type-Options nosniff;

ssl_dhparam /etc/ssl/certs/dhparam.pem;
```

#### Adjust the Nginx Configuration to Use SSL

Edit default.conf to make the following server blocks 

If the old default.conf is in place with jsut HTTP then replace;

```
server {
    listen       80;
    server_name  localhost mystore.local;

    #charset koi8-r;
    access_log  /var/log/nginx/host.access.log  main;
```

else/With;

```
server {
    listen 80 default_server;
    listen [::]:80 default_server;
    server_name  localhost mystore.local;
    return 302 https://$server_name$request_uri;
}

server {

    # SSL configuration

    listen 443 ssl http2 default_server;
    listen [::]:443 ssl http2 default_server;
    include conf.d/self-signed.conf;
    include conf.d/ssl-params.conf;

    #charset koi8-r;
    access_log  /var/log/nginx/host.access.log  main;

```

## Step 3:  Capture certificate files 

Download created SSL files from linux container and place within solution folders for build operations.

The following files (with paths) were be created and will need to be downloaded to the windows development host inside the solution folder structure for use in future builds.;

- /etc/ssl/private/myRootCA.key
- /etc/ssl/certs/myRootCA.pem
- /etc/ssl/certs/myRootCA.srl
- /etc/ssl/certs/myRootCA.pem
- /etc/ssl/certs/mystore.local.conf
- /etc/ssl/certs/mystore.local.crt
- /etc/ssl/certs/mystore.local.csr
- /etc/ssl/private/mystore.local.key
- /etc/ssl/certs/dhparam.pem

Each of these should be downloaded from the container file system to the windows development file system in the 'proxy' project folder.

Use the containers panel and the file tab. View\Other Windows\Containers. Select 'proxy.mystore.local' and select the 'Files' tab.

Navigate to each file and right click, then select 'Download'. 
Navigate to the 'Proxy' project folder as needed and click 'Save'.

When complete add all of the downloaded files to the solution in the Proxy virtual folder so that they are exposed to developer view.

They will be referenced in the Dockerfile with instructions following later to restore each file to its propeer place in each new image version.

The myRootCA files do not need to included in container images they are used to add to the Windows certificate hive.


### Add file COPIES to dockerfile
Now to deliver the files to subsequent image builds, add these file copy commands to the dockerfile after the existing copy commands.

```
COPY default.conf /etc/nginx/conf.d/default.conf
COPY self-signed.conf /etc/nginx/conf.d/self-signed.conf
COPY ssl-params.conf /etc/nginx/conf.d/ssl-params.conf
COPY dhparam.pem /etc/ssl/certs/dhparam.pem
COPY mystore.local.crt /etc/ssl/certs/mystore.local.crt
COPY mystore.local.key /etc/ssl/private/mystore.local.key
COPY index.html /usr/local/nginx/html/
```

### Add the root Certificate to the windows development host

In file explorer, locate the Proxy/myRootCA.pfx file.
Double click the file.
In the Certificate Import Wizard choose Local Machine and click Next.
Elevate privileges as requried.
At the select file dialog, which is already populated, just click next.
Enter the password 'password' or whatever you used above, and click next.
Select Place all certificates in the following store, click Browse, select 'Trusted Root Certification Authorities' then click OK.
Click Next
Click Finish.
The certificate will be installed.

### Clear down all existing containers and images
Perform a Clean solution from the Build menu
Remove all images made by the solution for a final rebuild from docker desktop or command line.

### Start solution and test
Start solution.

In a browser: Navigate to https://mystore.local and you will get an https session without any browser complaining.

Run the Postman test collection 

Note: If SSL certificate verification is off (in Postman File/Settings) these will now work. 
If its switched on, which checks the certificate authority chain you must switch on CA Certificates and add the myRootCA.pem in the SSL Certificates tab for it to continue to work.



# Container notes

For current details ples refer to the details in docker-compose project docker-compose.yml and docker-compose.override.yml

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


## ASP.NET Core

Applies to Container 1 and Container 2

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
Container name: store.mystore.local
hostname:		store.mystore.local
```
## Container 2

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
server_name  localhost mystore.local;
```

Sets a server name for thie NGINX proxy as either ```localhost``` or ```mystore.local```.

Means that in the following URL table ```localhost``` can be replaced with ```mystore.local``` but requires adjustment to ```\Windows\system32\drivers\etc\hosts``` file to direct to 127.0.0.1


#### location

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


##### Addressing scheme 


```
Http access 		        http://*  redirects to https://*
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

# Notes on linux for newbies

To temporarily add additional tooling to the container 

Connect to the console (which is root) via the Containers panel.
```
	apt update
```


## To add IP tools

Then either;

```
	apt-get install iputils-ping
```

and/or;

```
	apt-get install net-tools 
```

## For privileged access

```
apt install sudo
```
After which you can perform actions requiring root access by prefixing other commands with ```sudo```

## For editing files
```
apt install nano
```


