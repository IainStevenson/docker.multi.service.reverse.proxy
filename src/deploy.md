# deployment

Planning for (per environment);

* 1 Key vault
* 1 configuration
* 1 storage account
    * File shares used as volumes for mongodb
* 1 container registry
* 1 service app and plan for;
    * proxy/nginx
    * mongodb
    * identity
    * api
    * store
    * support

Needing certificates for each environment from lets-encrypt

* PROD
* TEST
* TEAM

Use self signed for local.

# Product name

miw [M]y[I]fo[W]orld

Need cloud environments for;

* PROD
* TEST
* TEAM
* Local - non cloud

# resource groups

* prod-miw-rg
* test-miw-rg
* team-miw-rg

# App Service

* identity-miw-as
* api-miw-as
* store-miw-as
* support-miw-as
* proxy-miw-as
* storage-miw-as

# Service Plan

* identity-miw-sp
* api-miw-sp
* store-miw-sp
* support-miw-sp
* proxy-miw-sp
* storage-miw-sp

# CI pipelines
 * triggered by commits to dev

# CD pipelines 
* triggered by CI -> dev release -> TEAM
* manual trigger dev -> TEST
* triggered by commits to main -> PROD