namespace Data.Model.Response
{
    public interface IResponseEntity : IEntity
    {
        string Etag { get; set; }
    }
}
