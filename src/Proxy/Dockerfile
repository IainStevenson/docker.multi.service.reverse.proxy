#
# Nginx Dockerfile
#
# https://github.com/dockerfile/nginx
#

# Pull base image.
FROM nginx:latest

# Define mountable directories.
VOLUME ["/etc/nginx/sites-enabled", "/etc/nginx/certs", "/etc/nginx/conf.d", "/var/log/nginx", "/var/www/html", "/usr/local/nginx/html"]

COPY default.conf /etc/nginx/conf.d/default.conf
COPY index.html /usr/local/nginx/html/

# Define default command.
ENTRYPOINT ["nginx"]
CMD ["-g", "daemon off;"]

# Expose ports.
EXPOSE 80
EXPOSE 443