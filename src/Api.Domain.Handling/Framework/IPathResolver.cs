namespace Api.Domain.Handling.Framework
{
    public interface IPathResolver
    {
        string PathOf(string path, Type forType);
    }
}
