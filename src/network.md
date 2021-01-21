# Network notes

The docker subnet network is agnostic about its actual run time IP Addresses and this 
solution leverages the automatic DNS feature of docker to provide 
container_to_container communications through thier deterministic hostnames.

Of course this does not help if kubernetes is in play which would require a more 
sophisticated service discovery technique.

## Network topology

This diagram illustrates the host and docker network setup.

```
.___________________________________________________________________________________________.
|                                                                                           |
|                    .___________________________________________________________________.  |
| Development host   | Docker        Front End             Back End                      |  |
| Network            | sub_network  ._____________.       ._________________________.    |  |
|                    |              |             |       |                   =     |    |  |
| Visual Studio      |              |             |       |  .___________.    |     |    |  |
| Browser            |              |             |       |->| store     |<-->|     |    |  |
| Postman            |              |             |       |  `___________'    |     |    |  |
| Console            |              |             |       |                   |     |    |  |
|                    |              | .________.  |       |  .___________.    |     |    |  |
|         https   ------------------->| proxy  |--https-->|->| identity  |<-->|     |    |  |
|                    |        ^     | `________'  |       |  `___________'    |     |    |  |
|         http    ------------'     |             |       |             http--|     |    |  |
|                    | 301 redirect |             |       |  .___________.    |     |    |  |
|                    |              |             |       |->| support   |<-->|     |    |  |
|                    |              |             |       |  `___________'    |     |    |  |
|                    |              `_____________'       |                   |     |    |  |
|                    |                                    |  .___________.    |     |    |  |
|  All protocols  ------ all ports -----------------x  -->|->| api       |<-->|     |    |  |
|                    |                                    |  `___________'    =     |    |  |
|                    |                                    `_________________________'    |  |
|                    `___________________________________________________________________'  |
`___________________________________________________________________________________________'
```

The docker network is a subnet set apart from its host, and can vary on each development host therefore ip addresses are non_deterministic.
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

Request URLS                                                    | Routes internally to
________________________________________________________________|________________________________________________________________________
https://mystore.local/identity/.well_lnown/openid_configuration | https://identity.mystore.local/identity/.well_lnown/openid_configuration
https://mystore.local                                           | https://store.mystore.local/store/
https://mystore.local/store                                     | https://store.mystore.local/store/
https://mystore.local/store/home                                | https://store.mystore.local/store/home
https://mystore.local/store/home/privacy                        | https://store.mystore.local/store/home/privacy
https://mystore.local/support                                   | https://support.mystore.local/support/
https://mystore.local/support/home                              | https://support.mystore.local/support/home
https://mystore.local/support/home/privacy                      | https://support.mystore.local/support/home/privacy
https://mystore.local/support/home/privacy                      | https://support.mystore.local/support/home/privacy
https://mystore.local/api/identity                              | https://api.mystore.local/api/identity
https://mystore.local/api/weatherforecast                       | https://api.mystore.local/api/weatherforecast

```

# Addressing scheme 


```
Http access                | http://*  redirects to https://*
________________________________________________________________________________________________________________
Default Store access       | https://mystore.local is served direct from container 1 index view
Store access via path      | https://mystore.local/store  is served direct from container 1 index view
Support access via path    | https://mystore.local/support is served direct from container 2 index view
```

URL inter_site redirects from Container 1 to Container 2 and vis_a_versa work as expected and intra site urls using controller actions work as expected

Example index.cshtml from WebApp1 shows a self referenceing controller action link and a standard domain relative ``` href="/support"``` link to the support site from the store;

```
@{
    ViewData["Title"] = "Home Page";
}

<div class="text_center">
    <h1 class="display_4">Welcome to the Store</h1>
    <p>Learn about <a class="nav_link text_dark" asp_area="" asp_controller="Home" asp_action="Privacy">our privacy policy</a></p>

    <p>Get some <a class="nav_link text_dark" href="/support">support</a></p>
</div>
```

