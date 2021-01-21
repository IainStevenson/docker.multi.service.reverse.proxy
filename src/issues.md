# Issues found

# OpenSSL

Either use the openssl you can download to a running container, or install git for windows on your host and use it from there. 

The provided solution command scripts expect Git For Windows to be installed.

The [certificates document](certificates.md) details how to leverage openssl inside a container.

# docker ports and exposure through NGINX

The intention is to limit attack surface to the NGINX proxy. Therefore all backend services are only available inside the docker network except the NGINX service which exposes port 80  and 443


# unified address space

Every url should have a bse of; https://mystore.local/
each service should have an additional base path added 
Identity = /identity
WebApp1 = /store
WebApp2 = /support
Api = /api

All set using app.UsePathBase("whatever");

# Requiring 



# Enabling ASP.NET core apps behind a reverse proxy

X-Forward headers

# docker files

# Generatng certificates

# devliering certificates

# host certificates

# root certificates

## linux (NGINX)

## ASP.NET (Linux)


# SSO issues with NGINX

# 500, 502 and 504 issues

Basically the Identity server headers are bigger tahtn NGINX accepts by default adn as additional claims are added the header grows and needs to be accomodated by the NGINX configuration setup.



