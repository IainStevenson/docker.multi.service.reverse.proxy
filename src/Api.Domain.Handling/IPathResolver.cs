namespace Domain.Handling
{
    public interface IPathResolver
    {
        string PathOf(string path, Type forType);
    }
}
