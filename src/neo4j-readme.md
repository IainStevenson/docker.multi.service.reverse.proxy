# NEO4J setup

Refering to [Setting up neo4j for SSL operations in a container](https://neo4j.com/developer/kb/setting-up-ssl-with-docker/)
the gen-host.cmd, docker-compose-override.yaml, and docker file were modified to install the Self-generated SSL certificates.


# Setup and configuration

the container is built and deployed to docker desktop by the docker-compose.yml file entry

```
graph:
    container_name: ${APP}.graph
    image: ${DOCKER_REGISTRY-}${APP}.graph:${TAG:-latest}
    hostname: graph.${APP_DOMAIN}
    build:
      context: .
      dockerfile: GraphDB/Dockerfile
    restart: always  
    ports:
      - "7474:7474"
      - "7473:7473"
      - "7687:7687"
    networks: 
      - backend
 ```
${APP}
${APP_DOMAIN}
${TAG}

Are set using the docker `.env` file.


The certificate is copied there by the entry in the `docker-compose-override.yml` file.

```
graph:
    environment:
      - NEO4J_AUTH${NEO4J_CREDENTIALS}
      - NEO4J_ACCEPT_LICENSE_AGREEMENT=yes
      - NEO4J_dbms_memory_pagecache_size=512m
      - NEO4J_dbms_default__advertised__address=graph.${APP_DOMAIN}
      #- NEO4J_dbms_ssl_policy_bolt_enabled=true
      #- NEO4J_dbms_ssl_policy_bolt_base__directory=certificates/bolt
      #- NEO4J_dbms_ssl_policy_bolt_private__key=myHost.key
      #- NEO4J_dbms_ssl_policy_bolt_myHost__certificate=myHost.crt
      #- NEO4J_dbms_ssl_policy_bolt_client__auth=NONE
      #- NEO4J_dbms_connector_bolt_tls__level=REQUIRED
      #- NEO4J_dbms_connector_https_enabled=true
      #- NEO4J_dbms_ssl_policy_https_enabled=true
      #- NEO4J_dbms_ssl_policy_https_base__directory=certificates/https
      #- NEO4J_dbms_ssl_policy_https_myHost__key=myHost.key
      #- NEO4J_dbms_ssl_policy_https_myHost__certificate=myHost.crt
      #- NEO4J_dbms_ssl_policy_https_client__auth=NONE
    volumes:
      - ${APPDATA}/GraphDb/Data:/data
      - ${APPDATA}/GraphDb/Logs:/logs
      - ${APPDATA}/GraphDb/ssl:/ssl

```

`NEO4J_dbms_default__advertised__address=graph.${APP_DOMAIN}` needs to be set to get neo4j to listen on the host dns address instead of localhost which the NGINX proxy cant use as it would not redirect to another container.

Without SSL The resulting listening address for the neo4j container is 

`http://graph.local.myinfo.world:7474/browser/`

with SSL it will be 

`https://graph.local.myinfo.world:7473/browser/`


## Dockerfile.



```
FROM neo4j:latest

# Allow trust of self-generated certificates to represent this host
RUN apt-get update
RUN apt-get install -y sudo
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates
```

