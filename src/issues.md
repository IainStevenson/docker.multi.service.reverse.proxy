# Issues found

Many problems occured in attempting this scenario the more difficult ones that held up progress are mentioned below.

There is a desire;
* to conform the domain URL scheme segemented by function.
* url addressing within MVC or Razor projects to work without fuss or any additional fiddling or configuration.
* to provide a single catch all development certificate for localhost AND the a wild card domain.
* for a single authentication and authorisation domain allowing for federation.
* to provide containerisation and orcehstrating multiple micro-services dedicated to one conceptual organisation task.


The following issues cropped up in providing those capabilities.


# Conformed address space

Intention: Every url should have a bse of; https://mystore.local/

Each service should have an additional base path added that should cope with internal relative Urls and respect eternal references across services.

Service  |   Base Path
_________|____________
Identity | /identity
WebApp1  | /store
WebApp2  | /support
Api      | /api

All of these were achieved by using app.UsePathBase("whatever"); in the Startup classes Configure method.
```
public void Configure(IApplicationBuilder app)
{
    app.UsePathBase("/identity");
    ...
}
``` 


# Authentication domain

[Identity server 4](https://identityserver.io/), via its /.well-known/openid-configuration document, provides an Issuer property which should be common to where ever you sign in from.

It should be the same as the authority set in each identity client.

The protection of the API relies on this and the audience value for access tokens and will validate each call via the identity server to check authenticity.

The following code in the Api will ensure this is correct.


```
        public void ConfigureServices(IServiceCollection services)
        {
            ...

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://mystore.local/identity";
                    options.Audience = "https://mystore.local/identity/resources";
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                    };
               });
            ...

        }
```

# OpenSSL

Either use the openssl you can download to a running container, or install git for windows on your host and use it from there. 

The provided solution command scripts expect Git For Windows to be installed.

The [certificates document](certificates.md) details how to leverage openssl inside a container.

# Docker ports and exposure through NGINX

The intention is to limit attack surface to the NGINX proxy. Therefore all backend services are only available inside the docker network except the NGINX service which exposes port 80  and 443


# Enabling ASP.NET core apps behind a reverse proxy

ASP.Net applications behind a reverse proxy rely on certain headers to be applied by the proxy and they need to be enabled in the middleware.

```
    app.UseForwardedHeaders(new ForwardedHeadersOptions
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
    });

```



# Docker files

Docker files NAMES are case sensative it should be

```
Dockerfile.
```

The context within the docker-compose should always be 

```
 build:
      context: .
```

# Generating certificates

As its a development environment LetsEncrypt is not available (its not a real domain)

So self-signed certificates are needed. some CMD scripts are provided to achieve that.

# Devilering certificates

Delivering certificates to the NGINX container ws straight forward as pere docuemntation for NGINX, however the ASP.NETE core certificates were a headache and you need to work with the ASP.NET core user secrets mechanism to succeed.

# Service host certificates

The Gen-host.cmd takes care of that for you and both provides the certificates signed by the root certificate and updates YOUR user secrets in YOUR project correctly.

# Root CA certificates

The Gen-Root.cmd creates one for you and; helps you get it registered on your development host in the correct place, and also provides it for use in the Dockerfile builds for the necessary services.

## Pure Linux (NGINX)

This was mostly derived from information found in the following resources, but modifed for this context. [Here](https://www.digitalocean.com/community/tutorials/how-to-create-a-self-signed-ssl-certificate-for-nginx-in-ubuntu-16-04) and [Here](https://www.obungi.com/2019/05/08/how-to-create-and-use-self-singed-certificates-in-an-ubuntu-docker-container-to-trust-external-resources/) 

## ASP.NET (Linux)

Two solutions needed.

One is taken care of by running the gen-host.cmd that creates the certificate, adds the necessary user secret settings, and places is ready for ASP.NET core to pick it up.

The other is adding in the root certificate to the host CA 

Directly after the base build command and expose in the Dockerfile,

```
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 443
```
adding the following snippet.
```
# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY Proxy/certificates/myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates
```

For some as yet unexplained reason adding the above snippet to the final section prior to the Entrypoint the certificates were not copied!

UPDATE: The reaons behind this is now clear after reading [this](https://docs.microsoft.com/en-us/visualstudio/containers/container-build?view=vs-2019)

The COPY commands, if placed later on, are applied to an intermediate image that is not used and effectively lost or outside the final image.

I think this can be finessed to exclude the curl and ca-certificates install's later, by performing the necessary COPY commands in base for adding the custom root CA cert
then moving the APT and RUN commands to the build phase, then COPY the modified /etc/ssl/certs/ca-certificates.crt result from build to final image. 

TO DO: try that out later but the size reduction will be minimal. This would be a production optimisation as curl is realy useful o have on teh image for debugging inter-container networking.

# SSO issues with NGINX

Not finding the signin-oidc endpoint on return from a login was solved by correctly setting the Base path AND fixing the 500 series errors as below.

# 500, 502 and 504 issues

Basically the Identity server headers coming back from a login are bigger than NGINX accepts, by default, and as additional claims are added over time, the header grows and needs to be accomodated by the NGINX configuration setup.

The following changes were made to the Proxy project  default.conf file.

```

# details from https://stackoverflow.com/questions/48964429/net-core-behind-nginx-returns-502-bad-gateway-after-authentication-by-identitys
# have been implemented to fix bad gateway problem returning from SSO.
# NOTE: This is in the http section via include from nginx.conf
#proxy_buffer_size   128k;
#proxy_buffers   4 256k;
#proxy_busy_buffers_size   256k;
#large_client_header_buffers 4 16k;
#fastcgi_buffers 16 16k;
#fastcgi_buffer_size 32k;

# after reading https://www.getpagespeed.com/server-setup/nginx/tuning-proxy_buffer_size-in-nginx
# changed to;
proxy_buffer_size 8k;

```

# Correctly using the bearer token for API calls in MVC clients

The folowing code snippet successfully sets up an HttpClient with the bearer token required by an ```[Authorize]``` protected  Method.

```
    var apiClient = new HttpClient();

    string accessToken = await HttpContext.GetTokenAsync("access_token");
    string refreshToken = await HttpContext.GetTokenAsync("refresh_token");

    apiClient.SetBearerToken(accessToken);

```

Although Dependency Injection with HttpClientFactory would be a better option, this works as a simple demostrator.

# Clear down all existing containers and images

Sometimes a debugging sesion will fail due to docker container or image name collisions.

This is a 'snafu' between VS and Docker so just clear them down and start again.
If necessary use docker desktop to stop the containers and to a 'cleanup' of the images and rebuild them.
or;
* Perform a Clean solution from the Build menu
* Remove all images made by the solution for a final rebuild from docker desktop or command line.
