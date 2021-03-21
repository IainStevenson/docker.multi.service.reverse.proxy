namespace Data.Model.Storage
{
    public interface IStoredEntity
    {
        IStorageMetadata Metadata { get; set; }
    }
}
