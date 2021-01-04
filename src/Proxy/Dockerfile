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
# TLS certificate files
COPY dhparam.pem /etc/ssl/certs/dhparam.pem
COPY mystore.local.crt /etc/ssl/certs/mystore.local.crt
COPY mystore.local.key /etc/ssl/private/mystore.local.key


# Define default command.
ENTRYPOINT ["nginx"]
CMD ["-g", "daemon off;"]

