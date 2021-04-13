# Notes on linux (for newbies like me)

To temporarily add additional tooling to the container 

Connect to the console (which is root) via the Containers panel.
```
apt update
```


## To add IP tools

Then either (for ping);

```
apt-get install iputils-ping
```

and/or (for netstat ifconfig, hostname dnsdomainname etc );

```
apt-get install net-tools 
```

## For privileged access

```
apt install sudo
```
After which you can perform actions requiring root access by prefixing other commands with ```sudo```

## For editing files

I recommend Nano for devs having a hard time with linux tools.

```
apt install nano
```

# List CA certificates

```
awk -v cmd='openssl x509 -noout -subject' ' /BEGIN/{close(cmd)};{print | cmd}' < /etc/ssl/certs/ca-certificates.crt
```

# Verify certificate 

```
openssl x509 -text -noout -in /etc/ssl/certs/whatever.crt 
```

# Verify intra-container communication via SSL

NOTE: This does not travel through the revrse-proxy.

```
apt update
apt-get install -y curl
curl https://identity.myInfo.local/identity/.well-known/openid-configuration
```

# Verify intra-container communication via SSL without certificate checking
```
apt update
apt-get install -y curl
curl -k https://identity.myInfo.local/identity/.well-known/openid-configuration
```


# Install a root certificate in a linux host
AFAIK There are two locations for certificates according to type (that I know to work), that roughly correspond to the 'Personal' and 'Trusted Root Certification Authorities' folders in Windows Certifcate manager.

```
/etc/ssl/certs
```

and

```
/usr/local/share/ca-certificates```

# Allow trust of certificates from other services

To allow the host default certificates to be trusted on each service that needs to do that.

Add this to the build (early) as needed.

```
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY Proxy/certificates/myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates
```