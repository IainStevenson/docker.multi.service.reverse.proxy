# Purpose

To build a secure `docker-compose`'d orchestration of a decoupled set of micro services running on Linux containers featuring;

* A secure Reverse Proxy (NGINX) guarding all other services.
* A secure `store` front-end Web site
* A secure `support` front-end Web site
* A common security domain and single sign on front-end Web site and service
	* (using Identity Server 4) allowing social identities and role based access
* A secure front-end/back-end `API` service 
	* WebApi with REST methods
	* Supporting 
	    * `If-*` headers as appropriate for concurrency control
	    * HATEOS
	    * `Swagger`
	    * `versioning`
* A secure back-end common persistence server 
	* using `MongoDB` with a common database and separate collections for each service
	* The ability to reconfigure to keep separate databases
	* Data at rest is not yet encrypted

# Getting started

- Required tooling: 
    - Install Visual Studio 2019 or above. Currently 2022.
    - Install Docker for windows
    - Install git for windows in its default location.
    - Download this repository and load it into visual studio.
* If git is anywhere else, or you have OpenSSL somewhere else, or modify ```src/Certificates/gen-vars.cmd``` to specify where to find `openssl.exe` 
- Set the startup to docker_compose using the right click menu on the solution to set startup project.
- Edit your hosts file
    - Add the following domain to the development environment private DNS by adding the following entry to your 
```%SystemRoot%\system32\drivers\etc\hosts``` file using any suitable text editor run as administrator.  Once that change is saved it is active immediately.

```
127.0.0.1 local.myInfo.world
```
	
- Install the self-signed domain trusted certificates to your development host, open a PowerShell, Terminal or command window in the ```src/Certificates``` folder.
	- Generate and install a root certificate execute ```./gen-root.cmd``` and follow instructions \* 
	- Generate default certificates for each micro service execute the ```gen-host.cmd``` and follow instructions.
- This will have created the following empty folders to persist MongoDB data across container run-times on your development host, by  executing the ```src\SetupLocalDB.CMD```

	```
	%APPDATA%\MongoDb\Data
	%APPDATA%\MongoDb\Logs
	```
-Execute ```user-secrets.cmd``` command script;

```
	parameters are: 
		action (SET|REMOVE)
		domain
		MONGO admin database name
		MONGO admin username
		MONGO admin password
		MONGO client connection string username
		MONGO client connection string password
		Identity shared client id
		Identity shared client secret
		[optional] Google login client id
		[optional] Google login secret
		[optional] Microsoft login client id
		[optional] Microsoft login secret
		[optional] GitHub login client id
		[optional] GitHub login secret
```		


* Using your already generated Google/ Facebook / Microsoft or GitHub Client and secrets in this command will setup your Visual Studio user-secrets to work with your development configuration. ```./user-secrets set local.myinfo.world admin admin storage storagepass Mvc secret googleid googlesecret microsoftid microsoftsecret githubid githubsecret```
* Note: ATM: if you use different database username and password it needs to also be reflected in ```src/.env``` and ```src/MongoDb/mongo-init.js``` settings.
* if you don't yet have a Google, Microsoft or GitHub external account sign in setup for your app then leave the optional parameters off and the identity server startup will not configure external authentication as an option and you can just use the test users. Find then in `Identity.Storage.SeedData`
* In google, Microsoft and GitHub developer consoles you will need to add in your allowed URL's as they would normally using the \*.domain They operate by redirect so they are picked up locally on the browser and still work according to the local development machine DNS via the hosts file.	

- Press F5.
	- If no browser appears, start one and navigate to https://local.myInfo.world and you will see the store site.
	- Navigate around, when you click Weather Forecast you will need to login, if logging in locally then use username: bob Password: bob, or use available social logins.
	- Alternatively or as well, import the ```local.myInfo.world.postman_collection.json``` file into postman and run the tests in the ```local.myInfo.world``` collection. This will acquire tokens from Identity Server and check basic MVC and API CRUD functionality.

\* I will convert this, side by side, as PowerShell later.

You can get more information about this solution and its journey at these locations.

* <a href="src/solution.md" target="_blank">Solution structure and issues.</a>
* <a href="src/certificates.md" target="_blank">SSL Self-Signed Certificates</a> 
* <a href="src/identity.md" target="_blank">Identity Server</a>
* <a href="src/Configuration.md" target="_blank">Configuration</a>
* <a href="src/docker.md" target="_blank">Docker</a>
* <a href="src/http.md" target="_blank">HTTP Url scheme</a>
* <a href="src/issues.md" target="_blank">General issues and solutions</a>
* <a href="src/linux.md" target="_blank">Linux for newbies</a>
* <a href="src/network.md" target="_blank">The network topology of the solution</a>
* <a href="src/oauth2.md" target="_blank">OAUTH2</a>






