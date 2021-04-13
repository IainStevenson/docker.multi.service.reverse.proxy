# Docker notes


Later on switch certificate updates to debug mode when letsencrypt is involved.
[Taken from this](https://docs.microsoft.com/en-us/visualstudio/containers/docker-compose-properties?view=vs-2019#customize-the-app-startup-process)

Customize the app startup process
You can run a command or custom script before launching your app by using the entrypoint setting, and making it dependent on the configuration. For example, if you need to set up a certificate only in Debug mode by running update-ca-certificates, but not in Release mode, you could add the following code only in docker-compose.vs.debug.yml:

yml

Copy
services:
  webapplication1:
    entrypoint: "sh -c 'update-ca-certificates && tail -f /dev/null'"
    labels:
      ...
If you omit the docker-compose.vs.release.yml or docker-compose.vs.debug.yml then Visual Studio generates one based on default settings.