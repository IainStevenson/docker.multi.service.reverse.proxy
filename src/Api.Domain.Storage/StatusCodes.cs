namespace Api.Domain.Storage
{
    /// <summary>
    /// Public static class to abstract System.Net.Http.StatusCode
    /// </summary>
    public enum HttpStatusCodes
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
