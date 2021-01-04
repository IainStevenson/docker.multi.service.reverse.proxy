# Self-Signing certificates for development use.

Swithing to TLS for the NGINX proxy in local/development requires self signed certificates or deep pockets for no real benefit. This is an excercise in how, not why, this can be done. The following proecedure was developed to create and use self-signed SSL certificates in NGINX.

This certificate configuration was taken and adapted to the current solutions build context, from [Here](https://www.digitalocean.com/community/tutorials/how-to-create-a-self-signed-ssl-certificate-for-nginx-in-ubuntu-16-04) and [Here](https://www.obungi.com/2019/05/08/how-to-create-and-use-self-singed-certificates-in-an-ubuntu-docker-container-to-trust-external-resources/) to work within a windows hosted Visual Studio multi-container docker-compose solution with multiple microservices fronted by NGINX.

there were many false starts developing this because the SAN settings detailed are quite particular when getting a clean certificate chain and host identification.

Rather than find OpenSSL for windows and install it locally, this procedure leverages the vanilla tools available as a Windows based VS developer to generate self signed certificates on the linux container, retrieve and store the certificate and configuration files on the windows development host for subsequent builds of those container images.

## Step 1: Create the SSL Certificate

## Access console and prepare container

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

Again, this requries manual input;

```
Enter Export Password: password
Verifying - Enter Export Password: password
```

Any password will do as you are not going to use this for production, are you?

That completes the creation of the self-signed root certificate. 


This can be used as a parent certificate to create many self signed certificates for other projects. 
It will, when installed on the local windows host machine under the 'Local Computer\Trusted Root Certification Authorities\Certificates' folder, automatically enable trust of those child certificates on your development machine, meaning the browser should not pose any certificate issues, not should there be problems in other tools like postman.


### Now create the service certificate
#### Create the key
```
openssl genrsa -out /etc/ssl/private/mystore.local.key 2048
```
#### Create a config file
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

#### Create a certificate signing request
```
openssl req -new -out /etc/ssl/certs/mystore.local.csr -key /etc/ssl/private/mystore.local.key -config /etc/ssl/certs/mystore.local.conf
```

#### Sign the certificate
```
openssl x509 -req -days 365 -CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key -CAcreateserial -extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:mystore.local,DNS:*.mystore.local")) -in /etc/ssl/certs/mystore.local.csr -out /etc/ssl/certs/mystore.local.crt
```

This signing command hooks it up to the previously created self-signed root CA via the ```-CA /etc/ssl/certs/myRootCA.pem -CAkey /etc/ssl/private/myRootCA.key```

NOTE: When using printf the back tick  “ “ disables the \n make sure its " "

The '-extensions SAN -extfile <(cat /etc/ssl/openssl.cnf <(printf "\n[SAN]\nsubjectAltName=DNS:mystore.local,DNS:*.mystore.local"))' part ensures that the subjectAlterativeNames are being kept in the singed certificate. Otherwhise you will get a irreversable error in Google Chrome.

### Create a strong Diffie-Hellman group, which is used in negotiating Perfect Forward Secrecy with clients

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


# Capture certificate files within solution folders for build operations.

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

# Clear down all existing containers and images
Perform a Clean solution from the Build menu
Remove all images made by the solution for a final rebuild from docker desktop or command line.

# Start solution and test
Start solution.

In a browser: Navigate to https://mystore.local and you will get an https session without any browser complaining.

Run the Postman test collection 

Note: If SSL certificate verification is off (in Postman File/Settings) these will now work. 
If its switched on, which checks the certificate authority chain you must switch on CA Certificates and add the myRootCA.pem in the SSL Certificates tab for it to continue to work.

