# Configuration

## Objectives

- Common configuration strategy for all host types
- Common configuration strategy for all environments
- Common code for handling reading of configuration variables into running code
- Change detection of configuration variables at run time
- Using secrets mechanisms for sensative information

# Environments

The standard .NET core IConfiguration environment definitions are as follows:

- Development: highly fluid locally hosted per developer environment
- Staging:	resonably fluid cloud or in premises hosted for;
-- Development team, 
-- QA team or Demo
-- User acceptance / pre-release 
- Production: reasonably static live production environment.

However you name your environments as you like.

# Configuration tasks
# Create local configuration variables

Run script to generate required configuration variables for the following tasks

Create the docker-compose .env file with the necessary environment variables


# Certificate creation and installation
Overwrite the src\Certificates\domain.conf
Create certificates if not existing
Create RootCA once per machine, 
check file exists locally if not create a new one
Remove existing  LocalMachine/Trusted Root Certifcation Authorities for presence of existing or past certificates that match CN but not Thnumbprint
Install existing CA Certificat as needed.
Create host certificates as needed
Deliver dhparam.em file to Build context folder 'src'
Copy host pfx certificates to %APPDATA%\ASP.NET\https folder

# MongoDB persistence

Create the %APPDATA%\MongoDb\Data folder
Create the %APPDATA%\MongoDb\Data folder
Create the src\MongoDb\mongo-init.js file

# Proxy
Generate the folowing files for the correct/each domain environment
streams-local.conf
sites-local.conf

* TODO: Work out how to use environment variables here

# ASP.Net Core services

Create user-secrets and reference the ASP.NET\https certificate and password
Add user-secrets for local configuration secrets
Mongo Connection String and database name


# Variables

$domainEnvironment
$domainName
$domainRoot


$certificatePassword


$mongoAdminUser
$mongoAdminPassword
$mongoUserName
$mongoUserPassword
$mongoPort

$googleClientId
$googleClientSecret
$microsoftClientId
$microsoftClientSecret
$microsoftClientId
$microsoftClientSecret
