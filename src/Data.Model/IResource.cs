namespace Data.Model
{
    public interface IResource
    {
        dynamic Content { get; set; }
        string Namespace { get; set; }
    }
}
