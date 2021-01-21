# Network notes

The docker subnet network is agnostic about its actual run time IP Addresses and this solution leverages the aautomatic DNS feature of docker to provide container to container communications through thier hostnames.

## Network topology
```
.-------------------------------------------------------------------------------------------.
|                                                                                           |
|                    .-------------------------------------------------------------------.  |
| Development host   | Docker        Front End             Back End                      |  |
| Network            | sub-network  .-------------.       .-------------------------.    |  |
|                    |              |             |       |                   =     |    |  |
| Visual Studio      |              |             |       |  .-----------.    |     |    |  |
| Browser            |              |             |       |->| store     |<-->|     |    |  |
| Postman            |              |             |       |  `-----------'    |     |    |  |
| Console            |              |             |       |                   |     |    |  |
|                    |              | .--------.  |       |  .-----------.    |     |    |  |
|         https   ---|--------------->| proxy  |--https-->|->| identity  |<-->|     |    |  |
|                    |        ^     | `--------'  |       |  `-----------'    |     |    |  |
|         http    ------------'     |             |       |             https-|     |    |  |
|                    | 301 redirect |             |       |  .-----------.    |     |    |  |
|                    |              |             |       |->| support   |<-->|     |    |  |
|                    |              |             |       |  `-----------'    |     |    |  |
|                    |              `-------------'       |                   |     |    |  |
|                    |                                    |  .-----------.    |     |    |  |
|  All protocols  ---|-- all ports -----------------x  -->|->| api       |<-->|     |    |  |
|                    |                                    |  `-----------'    =     |    |  |
|                    |                                    `-------------------------'    |  |
|                    `-------------------------------------------------------------------'  |
`-------------------------------------------------------------------------------------------'
```

The docker network is a subnet set apart from its host, and can vary on each development host therefore ip addresses are non-deterministic.
Dockers DNS capability is used by giving each container a deterministic dns name that can be used inside the docker subnet.
All services are provided with a discrete service certificate with the service host name as the CN.
All transport is encrypted. 
Performance is traded for security on the basis that security should never be undermined by performance considerations.
Any inadvertent exposure of backend services to the outside world can therefore fall back on TLS. (Strength in depth)

Outside world can only access the proxy at mystore.local where port 80 (http) redirects to port 443 (https)
Proxy can access all backend services only on port 443
All backend services can access all services only via port 443 (https) 
Certificates provided for each service and each has the development root CA certificate available to trust those services certificates.

# Application mapping in proxy configuration

With 4 defined externally useful application mappings of ```\```, ```\store\``` , ```\support\``` and ```\identity\```

With reference to  ```location  /store/ {``` in the proxy ```default.conf``` file, the trailing "\\" is important as it means the application path of ```\store``` is mapped along with all of its sub URL's.


The following URL's are supported correctly. 
The self referencing links inside the views of the micro services behave properly.
The following URL's are tested in this order.

```
Request URLS                                                    Routes internally to
https://mystore.local/identity/.well-lnown/openid-configuration http://identity.mystore.local/identity/.well-lnown/openid-configuration
https://mystore.local                                           http://store.mystore.local/store/
https://mystore.local/store                                     http://store.mystore.local/store/
https://mystore.local/store/home                                http://store.mystore.local/store/home
https://mystore.local/store/home/privacy                        http://store.mystore.local/store/home/privacy
https://mystore.local/support                                   http://support.mystore.local/support/
https://mystore.local/support/home                              http://support.mystore.local/support/home
https://mystore.local/support/home/privacy                      http://support.mystore.local/support/home/privacy
https://mystore.local/support/home/privacy                      http://support.mystore.local/support/home/privacy
```

# Addressing scheme 


```
Http access                 http://*  redirects to https://*
Default Store access        https://mystore.local is served direct from container 1 index view
Store access via path       https://mystore.local/store  is served direct from container 1 index view
Support access via path     https://mystore.local/support is served direct from container 2 index view
```

URL inter-site redirects from Container 1 to Container 2 and vis-a-versa work as expected and intra site urls using controller actions work as expected

Example index.cshtml from WebApp1 shows a self referenceing controller action link and a standard domain relative ``` href="/support"``` link to the support site from the store;

```
@{
    ViewData["Title"] = "Home Page";
}

<div class="text-center">
    <h1 class="display-4">Welcome to the Store</h1>
    <p>Learn about <a class="nav-link text-dark" asp-area="" asp-controller="Home" asp-action="Privacy">our privacy policy</a></p>

    <p>Get some <a class="nav-link text-dark" href="/support">support</a></p>
</div>
```

