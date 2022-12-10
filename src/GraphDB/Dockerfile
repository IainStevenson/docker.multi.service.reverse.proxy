#
# GraphDB Dockerfile
#
# https://github.com/dockerfile/mongodb
#
FROM neo4j:latest


# Allow trust of self-generated certificates to represent this host
RUN apt-get update
#RUN apt-get install -y sudo
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates
#RUN sudo chgrp -R 7474 /var/lib/neo4j/ssl 
#RUN sudo chmod -R g+rx /var/lib/neo4j/ssl
#

# NEO4J image has these
#EXPOSE 7473 7474 7687
#VOLUME [/data /logs /var/lib/neo4j/import /startup /plugins]
#WORKDIR /var/lib/neo4j
#ENTRYPOINT ["tini" "-g" "--" "/startup/docker-entrypoint.sh"]	
#CMD ["neo4j"]