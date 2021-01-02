# Self-Signing certificates for development use.

Documenting the proecedure followed to reatea nd use self-signed SSL certificates in NGINX.

This certificate configuration was taken and adapted to the current solutions build context, from;

https://www.digitalocean.com/community/tutorials/how-to-create-a-self-signed-ssl-certificate-for-nginx-in-ubuntu-16-04

To work within a windows hosted Visualo Studio multi-container docker-compose solution with multiple microservices fronted by NGINX.

It leverages the tools available as a Windows based VS developer to generate retrieve and store certificate and configuration files for subsequent builds of those container images.

As a result you dont need to seek out and install openssl on your windows host in order to achieve self-signing of certificates.

Build and start your vanilla nginx container using visual studio and start a terminal on that container. then enter the following commands once per solution.

## Step 1: Create the SSL Certificate

```
apt update
apt install sudo
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /etc/ssl/private/nginx-selfsigned.key -out /etc/ssl/certs/nginx-selfsigned.crt
```

respond to the prompts with the following information;
```
UK
London
London
mystore
local
mystore.local
admin@mystore.local
```

Dowload the files to the proxy project folder where they will be used in subsequent repeat builds of the same image by modifying the proxy dockerfile. As you go add them to the soution's proxy project folder as you download them.

Download by locating the files in the visual studio containers panel / files tab, and right clicking them and download file to start a download. Navigate to the proxy folder and click save.

/etc/ssl/certs/nginx-selfsigned.crt > src/proxy/nginx-selfsigned.crt
/etc/ssl/private/nginx-selfsigned.key > src/proxy/nginx-selfsigned.key


Now. create a strong Diffie-Hellman group, which is used in negotiating Perfect Forward Secrecy with clients

```
sudo openssl dhparam -out /etc/ssl/certs/dhparam.pem 2048
```

Again download the ```/etc/ssl/certs/dhparam.pem``` file you just created to the proxy project folder.

## Step 2: Configure Nginx to Use SSL


We will make a few adjustments to our proxy project for the NGINX configuration.

- We will create a configuration snippet containing our SSL key and certificate file locations.
- We will create a configuration snippet containing strong SSL settings that can be used with any certificates in the future.
- We will adjust our Nginx server blocks to handle SSL requests and use the two snippets above.

### Create a Configuration Snippet Pointing to the SSL Key and Certificate

(NOTE: Modified from source resource due to differing folder structure in NGINX)

```
sudo nano /etc/nginx/conf.d/self-signed.conf
```

Download this new file to the proxy project folder.

### Create a Configuration Snippet with Strong Encryption Settings

sudo nano /etc/nginx/conf.d/ssl-params.conf

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

```
server {
    listen       80;
    server_name  localhost mystore.local;

    #charset koi8-r;
    access_log  /var/log/nginx/host.access.log  main;
```

Replaced by;

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

now to deliver the files to subsequent image builds, add these file copy commands to the dockerfile after the existing copy commands.

```

COPY nginx-selfsigned.crt /etc/ssl/certs/nginx-selfsigned.crt
COPY dhparam.pem /etc/ssl/certs/dhparam.pem
COPY nginx-selfsigned.key /etc/ssl/private/nginx-selfsigned.key
COPY self-signed.conf /etc/nginx/conf.d/self-signed.conf
COPY ssl-params.conf /etc/nginx/conf.d/ssl-params.conf
```