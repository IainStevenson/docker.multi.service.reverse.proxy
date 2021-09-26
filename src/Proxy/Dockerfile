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

# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt 
RUN update-ca-certificates

# Copy Diffie-Hellman file
COPY dhparam.pem /etc/ssl/certs/dhparam.pem
# Copy local service certificate
COPY myHost.crt /etc/ssl/certs/myHost.crt
COPY myHost.key /etc/ssl/private/myHost.key

# TODO: Configuration
# use these to enable regenerating or adding any service certificate on the proxy host during development
#COPY myRootCA.crt /etc/ssl/certs/myRootCA.crt
#COPY myRootCA.key /etc/ssl/private/myRootCA.key
#COPY myRootCA.pem /etc/ssl/certs/myRootCA.pem
#COPY myRootCA.pfx /etc/ssl/certs/myRootCA.pfx
#COPY myRootCA.srl /etc/ssl/certs/myRootCA.srl

#COPY ./Proxy/environment.sh /docker-entrypoint.d/environment.sh
#RUN chmod +x /docker-entrypoint.d/environment.sh
COPY ./Proxy/templates /etc/nginx/templates
# Define default command.

#ENTRYPOINT ["nginx"]
#CMD ["-g", "daemon off;"]
# Knockout the entry point to allow the docker-entrypoint.sh to execute and configure envsubst
# And add the entire nginx command to be passed to docker-entrypoint.sh as a parameter.
CMD ["nginx", "-g", "daemon off;"]

