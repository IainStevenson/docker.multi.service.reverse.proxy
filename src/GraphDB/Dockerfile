#
# GraphDB Dockerfile
#
# https://github.com/dockerfile/mongodb
#
FROM neo4j:latest

# Resources:
# https://neo4j.com/docs/operations-manual/current/docker/
# https://neo4j.com/developer/kb/setting-up-ssl-with-docker/
# https://neo4j.com/developer/kb/setting-up-ssl-with-docker/
# For some of the certificate setup please review the Neo4J (GraphDB) section in Gen-Host.cmd

#
##/usr/local/share/ca-certificates/myRootCA.crt is where the certificate authority certificates reside
##/ssl/ is where neo4j expects the host certificates to reside
#
## Remember: When executing the host build context is the parent folder of this files folder.
# Allow trust of self-generated certificates to represent this host
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates
#RUN apt-get install -y sudo

# Copy Diffie-Hellman file
COPY dhparam.pem /etc/ssl/certs/dhparam.pem
# Copy local service certificate
COPY myHost.crt /etc/ssl/certs/myHost.crt
COPY myHost.key /etc/ssl/private/myHost.key

COPY myHost.key /ssl/bolt/myHost.key

