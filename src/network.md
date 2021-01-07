# Network notes

The docker subnet network is agnostic about its actual run time IP Addresses and this solution leverages the aautomatic DNS feature of docker to provide container to container communications through thier hostnames.

# Network topology
```
.-------------------------------------------------------------------------------------------.
|                                                                                           |
|                    .-------------------------------------------------------------------.  |
| Development host   | Docker                                                            |  |
| Network            | sub-network                                                       |  |
|                    |                                                                   |  |
|                    |                .--------.                                         |  |
|                 ---|--any port---x  | store  |--http--------------------------.        |  |
|                    |                `--------'             |                  |        |  |
|                    |                    ^ http             v                  v        |  |
|                    |                .--------.        .-----------.     .-----------.  |  |
|                 ---|-http/https---->| proxy  |--http->| identity  |     |   api     |  |  |
| Visual Studio      |                `--------'        `-----------'     `-----------'  |  |
| Browser            |                    v http             ^                  ^        |  |
| Postman            |                .---------.            |                  |        |  |
|                 ---|--any port---x  | support |--http-------------------------'        |  |
|                    |                `---------'                                        |  |
|                    |                                                                   |  |
|                    |                                                                   |  |
|                    |                                                                   |  |
|                    .-------------------------------------------------------------------'  |
|                                                                                           |
`-------------------------------------------------------------------------------------------'
```

#### location

With 4 defined location mappings of ```\```, ```\store``` , ```\support\``` and ```\identity\```

The following URL's are supported correctly. Also the self referencing links inside the views of the micro services behave properly.

```
Request URLS                                                    Routes internally to
https://mystore.local                                           http://store.mystore.local/store/
https://mystore.local/store                                     http://store.mystore.local/store/
https://mystore.local/store/home                                http://store.mystore.local/store/home
https://mystore.local/store/home/privacy                        http://store.mystore.local/store/home/privacy
https://mystore.local/support                                   http://support.mystore.local/support/
https://mystore.local/support/home                              http://support.mystore.local/support/home
https://mystore.local/support/home/privacy                      http://support.mystore.local/support/home/privacy
https://mystore.local/support/home/privacy                      http://support.mystore.local/support/home/privacy
https://mystore.local/identity/.well-lnown/openid-configuration http://identity.mystore.local/identity/.well-lnown/openid-configuration
```

With reference to  ```location  /store/ {```  the trailing "\\" is important as it means the application path of ```\store``` is mapped along with all of its sub URL's.


##### Addressing scheme 


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

