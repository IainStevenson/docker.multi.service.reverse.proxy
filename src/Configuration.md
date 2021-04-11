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



# Notes on changing domain

Regenerate the host certificates, the same root certiicate can be retained.
Change the database names in ```mongo-init-local.js``` 
Modify all of the ```appsettings.Development.json```

# Database names

If changing names with the same mongo instance the databses and collections will either;

- need to be created manually using Compass
- or strip out the files in the following folders and restart mongo;

```
${APPDATA}/MongoDb/Data
${APPDATA}/MongoDb/Logs
```


