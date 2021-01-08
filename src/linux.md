# Notes on linux for newbies (like me)

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
```
apt install nano
```


# List CA certificates

```
awk -v cmd='openssl x509 -noout -subject' ' /BEGIN/{close(cmd)};{print | cmd}' < /etc/ssl/certs/ca-certificates.crt
```

# Verify certificate 

```
openssl x509 -text -noout -in /etc/ssl/certs/mystore.local.crt 
```

# Verify intra-container communication via SSL
```
apt update
apt-get install -y curl
curl https://identity.mystore.local/identity/.well-known/openid-configuration
```

# Verify intra-container communication via SSL without certificate checking
```
apt update
apt-get install -y curl
curl -k https://identity.mystore.local/identity/.well-known/openid-configuration
```


# Verify identity.mystore.local
Start a container console and run these commands

```
apt update
apt-get install -y curl
openssl x509 -text -noout -in /etc/ssl/certs/identity.mystore.local.crt 
```



# Verify api.mystore.local
Start a container console and run these commands

```
apt update
apt-get install -y curl
openssl x509 -text -noout -in /etc/ssl/certs/api.mystore.local.crt 
curl -k https://identity.mystore.local/identity/.well-known/openid-configuration
curl https://identity.mystore.local/identity/.well-known/openid-configuration
awk -v cmd='openssl x509 -noout -subject' ' /BEGIN/{close(cmd)};{print | cmd}' < /etc/ssl/certs/ca-certificates.crt

```
