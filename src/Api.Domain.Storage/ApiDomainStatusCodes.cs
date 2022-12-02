namespace Api.Domain.Storage
{
    /// <summary>
    /// Public static class to abstract System.Net.HttpStatusCode and remove dependency on System.Net
    /// </summary>
    public enum ApiDomainStatusCodes
    {
        OK = 200,
        CREATED = 201,
        NOCONTENT = 204,
        NOTMODIFIED = 304,
        BADREQUEST = 400,
        NOTFOUND = 404,
        ALREADYGONE = 410,
        PRECONDITIONFAILED = 412
    }
}
