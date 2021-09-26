#!/usr/bin
set -eux

envsubst '${PROXY_DOMAIN}' < /etc/nginx/templates/streams.conf.template > /etc/nginx/sites-enabled/streams.conf
envsubst '${PROXY_DOMAIN}' < /etc/nginx/templates/sites.conf.template > /etc/nginx/sites-enabled/sites.conf
