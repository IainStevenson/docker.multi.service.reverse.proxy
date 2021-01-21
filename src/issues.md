# Issues found

Many problems occured in attempting this scenario the more difficult ones that held up progress are mentioned below.


# unified address space

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


# Unified domain

Identity server, via its /.well-known/openid-configuration document, provides an Issuer property which should be common to where ever you sign in from unless it is to be ignored.

We chose to require it but make it a common name that the API recieves from any client.

This was achieved by setting the following marked with ```< HERE``` in Identity server.

```
 public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddRequestResponseLoggingMiddlewareWithOptions(options => { options.LogSource = "Identity"; });

            var builder = services.AddIdentityServer(options =>
            {
                
                ...
                options.IssuerUri = "mystore"; // < HERE
            })
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryApiScopes(Config.ApiScopes)
                .AddInMemoryClients(Config.Clients)
                .AddTestUsers(TestUsers.Users);

            // not recommended for production - you need to store your key material somewhere secure
            builder.AddDeveloperSigningCredential();
        }
```


And the following in the Api.


```
        public void ConfigureServices(IServiceCollection services)
        {
            ...

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = "https://mystore.local/identity";
                    options.RequireHttpsMetadata = true;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // < -HERE
                        ValidateAudience = false,  // < -HERE
                    };
                });
            ...

        }
```

# OpenSSL

Either use the openssl you can download to a running container, or install git for windows on your host and use it from there. 

The provided solution command scripts expect Git For Windows to be installed.

The [certificates document](certificates.md) details how to leverage openssl inside a container.

# docker ports and exposure through NGINX

The intention is to limit attack surface to the NGINX proxy. Therefore all backend services are only available inside the docker network except the NGINX service which exposes port 80  and 443



# Enabling ASP.NET core apps behind a reverse proxy

X-Forward headers

# docker files

Docker files NAMES are case sensative it should be

```
Dockerfile.
```

The context within the docker-compose should always be 

```
 build:
      context: .
```

# Generatng certificates

As its a development environment LetsEncrypt is not available (its not a real domain)

So self-signed certificates are needed. some CMD scripts are provided to achieve that.

# Devliering certificates

Delivering certificates to the NGINX container ws straight forward as pere docuemntation for NGINX, however the ASP.NETE core certificates were a headache and you need to work with the ASP.NET core user secrets mechanism to succeed.

# host certificates

The Gen-host.cmd takes care of that for you and both provides the certificates signed by the root certificate and updates YOUR user secrets in YOUR project correctly.

# root certificates

The Gen-Root.cmd creates one for you and; helps you get it registered on your development host in the correct place, and also provides it for use in the Dockerfile builds for the necessary services.

## linux (NGINX)

This was mostly derived from information found in the following resources, but mofifed for this context.


## ASP.NET (Linux)

Two solutions needed.

One is taken care of by running the gen-host.cmd that creates the certificate, adds the necessary user secret settings, and places is ready for ASP.NET core to pick it up.

The other is adding in the root certificate to the host CA 

By adding this to the base build of the Dockerfile.

```
# Allow trust of certificates from other services
RUN apt-get update
RUN apt-get install -y curl
RUN apt-get install -y ca-certificates
COPY Proxy/certificates/myRootCA.crt /usr/local/share/ca-certificates/myRootCA.crt
RUN update-ca-certificates

```

# SSO issues with NGINX

Not finding the signin-oidc endpoint on return from a login was solved by correctly setting the Base path AND fixing the 500 series errors as below.

# 500, 502 and 504 issues

Basically the Identity server headers coming back from a login are bigger than NGINX accepts, by default, and as additional claims are added over time, the header grows and needs to be accomodated by the NGINX configuration setup.

The following changes were made to the Proxy project  default.conf file.

```

# details from https://stackoverflow.com/questions/48964429/net-core-behind-nginx-returns-502-bad-gateway-after-authentication-by-identitys
# have been implemented to fix bad gateway problem returning from SSO.
proxy_buffer_size   128k;
proxy_buffers   4 256k;
proxy_busy_buffers_size   256k;
large_client_header_buffers 4 16k;

```
And by adding these in each location setting.

```
        fastcgi_buffers 16 16k;
        fastcgi_buffer_size 32k;

```

# Correctly using the bearer token for inward looking API calls

The folowing code snippet successfully sets up an HhtpClient with the bearer token provided by a ```[Authorize]``` protected  Method.

```
          var apiClient = new HttpClient();

            string accessToken = await HttpContext.GetTokenAsync("access_token");
            string refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            apiClient.SetBearerToken(accessToken);

```

Although Dependency Injection with HttpClientFactory would be a better option, this works as a simple demostrator.


