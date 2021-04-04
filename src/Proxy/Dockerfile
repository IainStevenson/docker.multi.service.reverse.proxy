#
# Nginx Dockerfile
#
# https://github.com/dockerfile/nginx
#
# Pull base image.
FROM nginx:latest

# Define mountable directories.
VOLUME ["/etc/nginx", "/etc/nginx/sites-enabled", "/etc/nginx/certs", "/etc/nginx/private", "/etc/nginx/conf.d", "/var/log/nginx", "/var/www/html", "/usr/local/nginx/html"]

# Expose these ports to the outside network
EXPOSE 80 443

# Provide NGINX config files
COPY Proxy/nginx.conf /etc/nginx/nginx.conf
COPY Proxy/self-signed.conf /etc/nginx/conf.d/self-signed.conf
COPY Proxy/ssl-params.conf /etc/nginx/conf.d/ssl-params.conf
COPY Proxy/index.html /usr/local/nginx/html/
COPY Proxy/default.conf /etc/nginx/sites-enabled/sites-local.conf      # TODO: Configuration
COPY Proxy/stream.conf /etc/nginx/sites-enabled/streams-local.conf     # TODO: Configuration

# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY Proxy/certificates/myStoreRootCA.crt /usr/local/share/ca-certificates/myStoreRootCA.crt # TODO: Configuration
RUN update-ca-certificates

# Copy Diffie-Hellman file
COPY Proxy/certificates/dhparam.pem /etc/ssl/certs/dhparam.pem
# Copy local service certificate
COPY Proxy/certificates/mystore.crt /etc/ssl/certs/mystore.crt
COPY Proxy/certificates/myStore.key /etc/ssl/private/myStore.key

# TODO: Configuration
# use these to enable regenerating or adding any service certificate on the proxy host during development
#COPY Proxy/certificates/myStoreRootCA.crt /etc/ssl/certs/myStoreRootCA.crt
#COPY Proxy/certificates/myStoreRootCA.key /etc/ssl/private/myStoreRootCA.key
#COPY Proxy/certificates/myStoreRootCA.pem /etc/ssl/certs/myStoreRootCA.pem
#COPY Proxy/certificates/myStoreRootCA.pfx /etc/ssl/certs/myStoreRootCA.pfx
#COPY Proxy/certificates/myStoreRootCA.srl /etc/ssl/certs/myStoreRootCA.srl


# Define default command.
ENTRYPOINT ["nginx"]
CMD ["-g", "daemon off;"]

