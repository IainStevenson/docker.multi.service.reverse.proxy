# Network notes

The docker subnet network is agnostic about its actual run time IP Addresses and this 
solution leverages the automatic DNS feature of docker to provide 
container-to-container communications through their deterministic hostnames.

Container-to-container communications:
* (A) are (mediated) proxied by the proxy service if addresses use the the main domain  ```myInfo.local``` and application base paths. e,g, https://myInfo.local/api
* (B) are direct if the addresses use the actual sub-domain host name for the service e.b. https://api.myInfo.local/api


## Network topology

This diagram illustrates the host and docker network setup.

```
.___________________________________________________________________________________________.
|                                                                                           |
|                    .___________________________________________________________________.  |
| Development host   | Docker        Front End             Back End                      |  |
| Network            | sub_network  ._____________.       ._________________________.    |  |
|                    |              |             |       |                B  =     |    |  |
| Visual Studio      |              |             |       |  .___________.    |     |    |  |
| Browser            |              |             |   A   |->| store     |<-->|     |    |  |
| Postman            |              |             | https |  `___________'    |     |    |  |
| Console            |              |             |<------|                   |     |    |  |
|                    |              | .________.  |       |  .___________.    |     |    |  |
|         https   ------------------->| proxy  |--https-->|->| identity  |<-->|     |    |  |
|                    |        ^     | `________'  |       |  `___________'    |     |    |  |
|         http    ------------'     |             |       |             https-|     |    |  |
|                    | 301 redirect |             |       |  .___________.    |     |    |  |
|                    |              |             |       |->| support   |<-->|     |    |  |
|                    |              |             |       |  `___________'    |     |    |  |
|                    |              `_____________'       |                   |     |    |  |
|                    |                         |          |  .___________.    |     |    |  |
|  Other protocols------ and ports disabled-x  |       -->|->| api       |<-->|     |    |  |
|  Disabled          |                         |          |  `___________'    |     |    |  |
|                    |                         |          |                   |     |    |  |
|                    |                         |          |  .___________.    |     |    |  |
|  Except mongodb    |   Port   27017          `--------->|->| MongoDB   |<-->|     |    |  |
|                    |   For debugging only               |  `___________'    =     |    |  |
|                    |                                    `_________________________'    |  |
|                    `___________________________________________________________________'  |
`___________________________________________________________________________________________'
```

All services are provided with a discrete service certificate with ```localhost```  as the CN and all the requried domain and sub-domains as SAN (Subject Alternative Names) names. the default of ```localhost``` stops ASP.NET debuging from complaining about a localhost trusted certificate on startup.

All transport is encrypted. 

Clearly here, network transmission performance is reduced over a Front end TLS and backed http only scenario, due to increased TLS handshaking.

The decision here was to trade that performance drop for increased transport security on the basis that ```security should never be undermined by performance considerations```.

Any inadvertent exposure of backend services to the outside world can therefore fall back on TLS. (Strength in depth)

Outside world can only access the proxy at myInfo.local where port 80 (http) redirects to port 443 (https)

Proxy can access all backend services only on port 443

All backend services can access all services only via port 443 (https) 

Certificates provided for each service and each has the development root CA certificate available to trust those services certificates.

# Application mapping in proxy configuration

With 5 defined externally useful application mappings of;
```
/
/store/
/support/
/identity/
/api/
```

With reference to  the above locations, the trailing "/" is important as it means that 
for example the application path of ```/store``` is mapped along with all of its sub URL's.


The following URL's are supported correctly. 
The self referencing links inside the views of the micro services behave properly.
The following URL's are tested in this order.

```

Request URLS                                                    | Routes internally to
________________________________________________________________|________________________________________________________________________
https://myInfo.local/identity/.well_lnown/openid_configuration | https://identity.myInfo.local/identity/.well_lnown/openid_configuration
https://myInfo.local                                           | https://store.myInfo.local/store/
https://myInfo.local/store                                     | https://store.myInfo.local/store/
https://myInfo.local/store/home                                | https://store.myInfo.local/store/home
https://myInfo.local/store/home/privacy                        | https://store.myInfo.local/store/home/privacy
https://myInfo.local/support                                   | https://support.myInfo.local/support/
https://myInfo.local/support/home                              | https://support.myInfo.local/support/home
https://myInfo.local/support/home/privacy                      | https://support.myInfo.local/support/home/privacy
https://myInfo.local/support/home/privacy                      | https://support.myInfo.local/support/home/privacy
https://myInfo.local/api/identity                              | https://api.myInfo.local/api/identity
https://myInfo.local/api/weatherforecast                       | https://api.myInfo.local/api/weatherforecast

```

# Addressing scheme 


```
Http access                | http://*  redirects to https://*
________________________________________________________________________________________________________________
Default Store access       | https://myInfo.local is served direct from container 1 index view
Store access via path      | https://myInfo.local/store  is served direct from container 1 index view
Support access via path    | https://myInfo.local/support is served direct from container 2 index view
```

URL inter_site redirects from Container 1 to Container 2 and vis_a_versa work as expected and intra site urls using controller actions work as expected

Example index.cshtml from Store shows a self referenceing controller action link and a standard domain relative ``` href="/support"``` link to the support site from the store;

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

