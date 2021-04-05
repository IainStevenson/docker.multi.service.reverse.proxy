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
# TODO: Configuration +++
COPY Proxy/sites-local.conf /etc/nginx/sites-enabled/sites-local.conf      
COPY Proxy/streams-local.conf /etc/nginx/sites-enabled/streams-local.conf     
# TODO: Configuration ---

# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
# TODO: Configuration +++
COPY Proxy/certificates/myInfoRootCA.crt /usr/local/share/ca-certificates/myInfoRootCA.crt 
# TODO: Configuration ---
RUN update-ca-certificates

# Copy Diffie-Hellman file
COPY Proxy/certificates/dhparam.pem /etc/ssl/certs/dhparam.pem
# Copy local service certificate
COPY Proxy/certificates/myInfo.crt /etc/ssl/certs/myInfo.crt
COPY Proxy/certificates/myInfo.key /etc/ssl/private/myInfo.key

# TODO: Configuration
# use these to enable regenerating or adding any service certificate on the proxy host during development
#COPY Proxy/certificates/myInfoRootCA.crt /etc/ssl/certs/myInfoRootCA.crt
#COPY Proxy/certificates/myInfoRootCA.key /etc/ssl/private/myInfoRootCA.key
#COPY Proxy/certificates/myInfoRootCA.pem /etc/ssl/certs/myInfoRootCA.pem
#COPY Proxy/certificates/myInfoRootCA.pfx /etc/ssl/certs/myInfoRootCA.pfx
#COPY Proxy/certificates/myInfoRootCA.srl /etc/ssl/certs/myInfoRootCA.srl


# Define default command.
ENTRYPOINT ["nginx"]
CMD ["-g", "daemon off;"]

