# Security

From a security perspective the primary objectives are;

* To have a dedicated authentication and authorization domain 
* Implement FULL SSL for encryption in motion. 
 	* implementation for all services in all environments.  
 	* Implement encryption at rest as can be achieved with community edition MongoDB
* Avoid having to use `localhost` and multiple ports as part of the domain host. 
 	* `localhost` used in multi-service container configurations is worse than meaningless, relies on ports, then suddenly it becomes a blocker especially with respect to IdentityServer.
 	* Avoiding the issue that without a certificate that supports `localhost` in development .NET services blow up on startup.

# Identity, Identity Server and authorisation notes

Identity server provides an authorisation domain specif to this set of micro services and allows integration with many social identity providers through OAUTH and OpenID.

The main project for that is ```Identity``` and is a custom Identity Server 4 for .NET core.


# Identity Server. API and API Test Client


To get these working set identity and API to use the self hosting startup option, then set multipe project startup of Identity, Api and API test client in that order of execution.


Following the steps in [the Identity Server 4 Documentation](https://identityserver4.readthedocs.io/en/latest/quickstarts/0_overview.html)

Notes: Project name changes and url path modifications made.

Note: Microsoft.AspNetCore.Authentication.JwtBearer must be set to version 3.1.10 as the latest versions only support .NET Framework 5!

Note: There are various differences in teh Api: Statup.cs such as the following which were left in place.
```
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
```
Api Test Client uses enhanced colours, Green for headings, yellow for Input, White for output Grey for Information on program activity, red for errors.
# Google 
# Setup is straight forward

Ensure you capture the Client Secret while you can

# Microsoft Account

## Setup

- Go through the normal [Account setup](https://portal.azure.com)
- Add your requried URI's with the ```/identity/signin-microsoft``` suffix
- Obtain the 'Application (Client) ID' from the Overview for the configuration ClientId
- Add a new secret and get the Secret 'Value' as the  configuration ClientSecret

Then modify the Manifest as below;

```
"accessTokenAcceptedVersion": 2,
"signInAudience": "AzureADandPersonalMicrosoftAccount"
```

As found in [this post](https://stackoverflow.com/questions/63924622/getting-unauthorized-client-when-trying-to-login-using-microsoft-account)


