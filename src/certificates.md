# Overview

Most of this is historically correct during development but has been superceeded by events.

I am leaving it intact to provide background information and ways of doing things that may be of use in future.

The processes described here are superceeded by the Gen-Root.cmd and Gen-host.cmd in the Proxy project.

# Using LetsEncrypt
https://dev.to/herocod3r/configuring-ssl-on-an-aspnet-core-docker-container-1c9l
https://docs.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-5.0
https://runnable.com/blog/how-to-use-lets-encrypt-on-kubernetes
https://www.digitalocean.com/community/tutorials/how-to-secure-a-containerized-node-js-application-with-nginx-let-s-encrypt-and-docker-compose
https://pentacent.medium.com/nginx-and-lets-encrypt-with-docker-in-less-than-5-minutes-b4b8a60d3a71



# Certificate and SSL configuration notes

The self-signed certificates are organised into a root CA to authorise many service certificates.

The root CA is installed in any trusting host, including the development host for the docker network.

The trusted host recieves its host identification certificate as required during the build via dockerfile instructions.

The certificate generation, configuration and consumption is split into the following cases.

- Create self-signed root CA
- Capture the created root certificate files back to the Proxy project for future builds
- Create service certificate off of the Root CA
- Capture the created service certificate files back to the Proxy project for future builds
- Intall the Root CA on the development host (Once)
- Install the service certificate in the required services (each build)
- Install the Root CA in the trusting service in a container-to-container network case. (API -> Identity)

Create the service certificate to cover each of the following service hostnames.

local.myInfo.world
*.local.myInfo.world

The ALT wildcard name of \*.local.myInfo.world will allow the same certificate to be used for all backend services having a dnsname of local.myInfo.world.


# Self-Signing certificates for development use.

Swithing to TLS for the NGINX proxy in local/development requires self signed certificates or deep pockets for no real benefit. This is an excercise in how, not why, this can be done. The following proecedure was developed to create and use self-signed SSL certificates in NGINX.

This certificate configuration was taken and adapted to the current solutions build context, from [Here](https://www.digitalocean.com/community/tutorials/how-to-create-a-self-signed-ssl-certificate-for-nginx-in-ubuntu-16-04) and [Here](https://www.obungi.com/2019/05/08/how-to-create-and-use-self-singed-certificates-in-an-ubuntu-docker-container-to-trust-external-resources/) to work within a windows hosted Visual Studio multi-container docker-compose solution with multiple microservices fronted by NGINX.

there were many false starts developing this because the SAN settings detailed are quite particular when getting a clean certificate chain and host identification.

Rather than find OpenSSL for windows and install it locally, this procedure leverages the vanilla tools available as a Windows based VS developer to generate self signed certificates on the linux container, retrieve and store the certificate and configuration files on the windows development host for subsequent builds of those container images.

# Step 1: Create the SSL Certificate

# Access console and prepare container

Start the project using docker-compose as the startup project.

Then in the menu if its not already running; View\Other Windows\Containers

Select ```proxy.local.myInfo.world```

Use the menu icons to start a console.

Then enter the following and subsequent commands.

```
apt update
apt install nano
```

# Create the common development root certificate

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

# Convert the root Certificate to PFX to be able to import it into Windows

```
openssl pkcs12 -export -inkey /etc/ssl/private/myRootCA.key -in /etc/ssl/certs/myRootCA.pem -out /etc/ssl/certs/myRootCA.pfx
```

Again, this requires manual input;

```
Enter Export Password: password
Verifying - Enter Export Password: password
```

Any password will do as you are not going to use this for production, are you?


# Convert the CA certificate to CRT to be able to import it into Ubuntu
This is needed to provide the proxy or any other docker container with an ability to trust any certificate emanating from another container
```
openssl pkcs12 -in /etc/ssl/certs/myRootCA.pfx -clcerts -nokeys -out /usr/local/share/ca-certificates/myRootCA.crt
```


Again, this requires manual input;

```
Enter Import Password: password
```


Capture the output during the generation session by downloading the /usr/local/share/ca-certificates/myRootCA.crt to the Proxy project folder.

Additionally, to implement this CA on a container, include the following in the Dockerfile for that service;

```
# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates
```


That completes the creation of the self-signed root certificate. 

This can be used as a parent certificate to create many self signed certificates for other projects. 
It will, when installed on the local windows host machine under the 'Local Computer\Trusted Root Certification Authorities\Certificates' folder, automatically enable trust of those child certificates on your development machine, meaning the browser should not pose any certificate issues, nor should there be problems in other tools like postman.



# Now create the service certificate
# Create the key
One for each host certificate needed.
```
openssl genrsa -out /etc/ssl/private/local.myInfo.world.key 2048
openssl genrsa -out /etc/ssl/private/api.local.myInfo.world.key 2048
openssl genrsa -out /etc/ssl/private/identity.local.myInfo.world.key 2048
```
# Create a config file
Create a config file : /etc/ssl/certs/local.myInfo.world.conf
Containing;

```
touch /etc/ssl/certs/local.myInfo.world.conf
touch /etc/ssl/certs/api.local.myInfo.world.conf
touch /etc/ssl/certs/identity.local.myInfo.world.conf
nano /etc/ssl/certs/local.myInfo.world.conf
nano /etc/ssl/certs/api.local.myInfo.world.conf
nano /etc/ssl/certs/identity.local.myInfo.world.conf
```

Paste in the following then Control-x and save the file.

```
[req]
distinguished_name = local.myInfo.world
req_extensions = v3_req
prompt = no
[local.myInfo.world]
C = UK
ST = London
L = London
O = myInfo
OU = local
CN = local.myInfo.world
[v3_req]
keyUsage = keyEncipherment, dataEncipherment
extendedKeyUsage = serverAuth
subjectAltName = @alt_names
[alt_names]
DNS.1 = local.myInfo.world
DNS.2 = *.local.myInfo.world
```

The critical (specific) elements here are; ```distinguished_name = local.myInfo.world``` ```CN = local.myInfo.world``` and;
```
[alt_names]
DNS.1 = *.local.myInfo.world
```

In which there is a dependency in the signing section below.

# Create a certificate signing request
```
openssl req -new -out /etc/ssl/certs/local.myInfo.world.csr -key /etc/ssl/private/local.myInfo.world.key -config /etc/ssl/certs/local.myInfo.world.conf
openssl req -new -out /etc/ssl/certs/api.local.myInfo.world.csr -key /etc/ssl/private/api.local.myInfo.world.key -config /etc/ssl/certs/api.local.myInfo.world.conf
openssl req -new -out /etc/ssl/certs/identity.local.myInfo.world.csr -key /etc/ssl/private/identity.local.myInfo.world.key -config /etc/ssl/certs/identity.local.myInfo.world.conf
```

# Sign the certificate
```
openssl x509 -req -days 365 -CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key -CAcreateserial -extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:local.myInfo.world,DNS:*.local.myInfo.world")) -in /etc/ssl/certs/local.myInfo.world.csr -out /etc/ssl/certs/local.myInfo.world.crt
openssl x509 -req -days 365 -CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key -CAcreateserial -extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:api.local.myInfo.world,DNS:*.local.myInfo.world")) -in /etc/ssl/certs/api.local.myInfo.world.csr -out /etc/ssl/certs/api.local.myInfo.world.crt
openssl x509 -req -days 365 -CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key -CAcreateserial -extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:identity.local.myInfo.world,DNS:*.local.myInfo.world")) -in /etc/ssl/certs/identity.local.myInfo.world.csr -out /etc/ssl/certs/identity.local.myInfo.world.crt
```

This signing command hooks it up to the previously created self-signed root CA via the ```-CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key```

NOTE: When using printf the back tick  � � disables the \n make sure its " "

The '-extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:local.myInfo.world,DNS:*.local.myInfo.world"))' part ensures that the subjectAlterativeNames are being kept in the singed certificate. Otherwhise you will get a irreversable error in Google Chrome.

# Create a strong Diffie-Hellman group

Which is used in negotiating Perfect Forward Secrecy with clients

This isn't necessary to make TLS work in this context but when you dive into it its a good thing to know about and do anyway.

```
openssl dhparam -out /etc/ssl/certs/dhparam.pem 2048
```

# Step 2: Configure Nginx to Use SSL


We will make a few adjustments to our proxy project for the NGINX configuration.

- We will create a configuration snippet containing our SSL key and certificate file locations.
- We will create a configuration snippet containing strong SSL settings that can be used with any certificates in the future.
- We will adjust our Nginx server blocks to handle SSL requests and use the two snippets above.

# Create a Configuration Snippet Pointing to the SSL Key and Certificate

(NOTE: Modified from source resource due to differing folder structure in NGINX)

```
nano /etc/nginx/conf.d/self-signed.conf
```

Enter the following and save the file.

```
ssl_certificate /etc/ssl/certs/local.myInfo.world.crt;
ssl_certificate_key /etc/ssl/private/local.myInfo.world.key;
```

# Create a Configuration Snippet with Strong Encryption Settings

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
# add_header Strict-Transport-Security "max-age=63072000; includeSubdomains; preload";
add_header Strict-Transport-Security "max-age=63072000; includeSubdomains";
add_header X-Frame-Options DENY;
add_header X-Content-Type-Options nosniff;

ssl_dhparam /etc/ssl/certs/dhparam.pem;
```

# Adjust the Nginx Configuration to Use SSL

Edit default.conf to make the following server blocks 

If the old default.conf is in place with jsut HTTP then replace;

```
server {
    listen       80;
    server_name  localhost local.myInfo.world;

    #charset koi8-r;
    access_log  /var/log/nginx/host.access.log  main;
```

else/With;

```
server {
    listen 80 default_server;
    listen [::]:80 default_server;
    server_name  localhost local.myInfo.world;
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

# Step 3:  Capture certificate files 

Download created SSL files from linux container and place within solution folders for build operations.

The following files (with paths) were be created and will need to be downloaded to the windows development host inside the solution folder structure for use in future builds.;

- /usr/share/ca-certificates/myRootCA.crt

- /etc/ssl/certs/myRootCA.pem
- /etc/ssl/certs/myRootCA.srl
- /etc/ssl/certs/myRootCA.pem
- /etc/ssl/certs/local.myInfo.world.conf
- /etc/ssl/certs/local.myInfo.world.crt
- /etc/ssl/certs/local.myInfo.world.csr

- /etc/ssl/private/myRootCA.key
- /etc/ssl/private/local.myInfo.world.key

- /etc/ssl/certs/dhparam.pem

Each of these should be downloaded from the container file system to the windows development file system in the 'proxy' project folder.

Use the containers panel and the file tab. View\Other Windows\Containers. Select 'proxy.local.myInfo.world' and select the 'Files' tab.

Navigate to each file and right click, then select 'Download'. 
Navigate to the 'Proxy' project folder as needed and click 'Save'.

When complete add all of the downloaded files to the solution in the Proxy virtual folder so that they are exposed to developer view.

They will be referenced in the Dockerfile with instructions following later to restore each file to its propeer place in each new image version.

The myRootCA files do not need to included in container images they are used to add to the Windows certificate hive.


# Add file COPIES to proxy dockerfile
Now to deliver the files to subsequent image builds, add these file copy commands to the dockerfile after the existing copy commands.

```
# NGINX config files
COPY default.conf /etc/nginx/conf.d/default.conf
COPY self-signed.conf /etc/nginx/conf.d/self-signed.conf
COPY ssl-params.conf /etc/nginx/conf.d/ssl-params.conf
COPY index.html /usr/local/nginx/html/

# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates
# Copy Diffie-Hellman file
COPY dhparam.pem /etc/ssl/certs/dhparam.pem
# Copy local service certificate
COPY local.myInfo.world.crt /etc/ssl/certs/local.myInfo.world.crt
COPY local.myInfo.world.key /etc/ssl/private/local.myInfo.world.key

```

# Add the root Certificate to the windows development host

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


# Useful resources

.NET 8.0 has changed the ball game


https://stackoverflow.com/questions/77063237/how-to-configure-ssl-certificate-in-docker-container

https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0

https://learn.microsoft.com/en-us/aspnet/core/security/authentication/certauth?view=aspnetcore-8.0



```
# Generate Certificate using OpenSSL
# Assume Git is installed, which includes OpenSSL client

write-host "STARTING GEN CERTIFICATE"

$outputFile = (Get-Item .).FullName
write-host "Working Directory: " $outputFile

# NOTE: single '' is used by powershell, versus "" used by OS
$openssl = 'C:\Program Files\Git\usr\bin\openssl.exe'
write-host "OpenSSL Executable: " $openssl

# UPDATE THIS: step options: 1-7, CA, SR, ALL
$step = 7

# UPDATE THIS: hostname to generate certificate for
$hostname = "service02"

# local variables
$caKey = "ca.key"
$caCert = "ca.crt"
$serverKey = "$hostname.key"
$serverCsr = "$hostname.csr"
$serverCert = "$hostname.crt"
$serverPfx = "$hostname.pfx"

# UPDATE THIS: certificate details
$certSubj = '"/C=CA/ST=BritishColumbia/L=Vancouver/O=MyCo/OU=MyCo/CN=MyCo.local"'
$certExt = '"subjectAltName=DNS:myco.local"'

$serverSubj = "/C=CA/ST=BritishColumbia/L=Vancouver/O=MyCo/OU=MyCo/CN=$hostname.local"
$serverExt = "subjectAltName=DNS:$hostname.local"

# show settings
if (($step -eq 0) -or ($step -eq "CA") -or ($step -eq "SR") -or ($step -eq "ALL"))
{
    write-host "Settings..."
    write-host "step $step"
    write-host $hostname
    write-host $caKey $caCert
    write-host $serverKey $serverCsr $serverCert
    write-host $serverExt
    
# TEST command will execute
    #& $openssl
    #& $openssl help
}

# Generate CA private key
if (($step -eq 1) -or ($step -eq "CA") -or ($step -eq "ALL"))
{
    & $openssl genrsa -out $caKey 4096
}

# Generate CA certificate
if (($step -eq 2) -or ($step -eq "CA") -or ($step -eq "ALL"))
{
    & $openssl req -x509 -new -nodes -days 365 -sha256 -key $caKey -out $caCert -subj "$certSubj" -addext "$certExt"
}

# Generate CSR for CA
if (($step -eq 3) -or ($step -eq "CA") -or ($step -eq "ALL"))
{
    #& $openssl req -newkey rsa:4096 -keyout $caprivatekey -out cert.csr -sha256 -days 365 -nodes -subj "$certSubj" -addext "$certExt"
}

# Generate Server Private Key
if (($step -eq 4) -or ($step -eq "SR") -or ($step -eq "ALL"))
{
    & $openssl genrsa -out $serverKey 4096
}

# Generate Server CSR
if (($step -eq 5) -or ($step -eq "SR") -or ($step -eq "ALL"))
{
    & $openssl req -new -key $serverKey -out $serverCsr -sha256 -subj "$serverSubj" -addext "$serverExt"
}

# Generate Server SSL Certificate along with certificates serial
if (($step -eq 6) -or ($step -eq "SR") -or ($step -eq "ALL"))
{
    & $openssl req -x509 -days 365 -sha256 -CA $caCert -CAkey $caKey -in $serverCsr -out $serverCert -addext $serverExt -copy_extensions copyall
}

# Generate Server PFX certificate
if (($step -eq 7) -or ($step -eq "SR") -or ($step -eq "ALL"))
{
    write-host "Generating PFX certificate: you will be prompted to enter a password and confirm it"
    & $openssl pkcs12 -export -in $serverCert -inkey $serverKey -out $serverPfx
    # -certfile root?
}

```