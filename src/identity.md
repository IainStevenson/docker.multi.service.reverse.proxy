# Identity, Identity SErver and authorisation notes

Identity server provides an authorisation domain specif to this set of micro services and allows integration with many social identity providers through OAUTH and OpenID.

The main project for that is ```Identity``` and is a custom Identity Server 4 for .NET core.


# Identity Server. API and API Test Client


To get these working set identity and API to use the self hosting startup option, then set multipe project startup of Identity, Api and API test client in that order of execution.


Following the steps in [the Identity Server 4 Documentation](https://identityserver4.readthedocs.io/en/latest/quickstarts/0_overview.html)

Notes: Project name changes and url path modifications made.

Note: Microsoft.AspNetCore.Authentication.JwtBearer must be set to version 3.1.10 as the latest versions only support .NET Framework 5!

Note: Tehre are various differences in teh Api: Statup.cs such as the following which were left in place.
```
if (env.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
```
Api Test Client uses enhanced colours, Green for headings, yellow for Input, White for output Grey for Information on program activity, red for errors.

