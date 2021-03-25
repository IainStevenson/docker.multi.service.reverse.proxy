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



