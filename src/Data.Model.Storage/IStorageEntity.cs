namespace Data.Model.Storage
{
    public interface IStorageEntity : IEntity
    {
        string Etag { get; set; }
    }
}
