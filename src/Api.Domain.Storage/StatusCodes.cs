namespace Api.Domain.Storage
{
    /// <summary>
    /// Public static class to abstract System.Net.Http.StatusCode
    /// </summary>
    public static class StatusCodes
    {
        public const int OK  = 200;
        public const int CREATED = 201;
        public const int NOCONTENT = 204;
        public const int NOTMODIFIED =304;
        public const int BADREQUEST = 400;
        public const int NOTFOUND = 404;
        public const int ALREADYGONE = 410;
        public const int PRECONDITIONFAILED = 412;
    }
}
