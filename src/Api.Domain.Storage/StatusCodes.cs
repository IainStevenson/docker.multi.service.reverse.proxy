namespace Api.Domain.Storage
{
    /// <summary>
    /// Public static class to abstract System.Net.Http.StatusCode
    /// </summary>
    public static class StatusCodes
    {
        public const int BADREQUEST = 400;
        public const int NOTFOUND = 404;
        public const int ALREADYGONE = 410;
        public const int PRECONDITIONFAILED = 412;
        public const int NOCONTENT = 204;

        public static int OK  = 200;

        public static int CREATED = 201;

        public static int NOTMODIFIED =304;
    }
}
