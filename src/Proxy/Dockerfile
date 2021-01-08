#
# Nginx Dockerfile
#
# https://github.com/dockerfile/nginx
#

# Pull base image.
FROM nginx:latest

# Define mountable directories.
VOLUME ["/etc/nginx/sites-enabled", "/etc/nginx/certs", "/etc/nginx/private", "/etc/nginx/conf.d", "/var/log/nginx", "/var/www/html", "/usr/local/nginx/html"]

# Expose ports.
EXPOSE 80 443

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
COPY mystore.local.crt /etc/ssl/certs/mystore.local.crt
COPY mystore.local.key /etc/ssl/private/mystore.local.key

# use these to enable regenerating or adding any service certificate on the proxy host during development
COPY myRootCA.crt /etc/ssl/certs/myRootCA.crt
COPY myRootCA.key /etc/ssl/private/myRootCA.key
COPY myRootCA.pem /etc/ssl/certs/myRootCA.pem
COPY myRootCA.pfx /etc/ssl/certs/myRootCA.pfx
COPY myRootCA.srl /etc/ssl/certs/myRootCA.srl


# Define default command.
ENTRYPOINT ["nginx"]
CMD ["-g", "daemon off;"]

