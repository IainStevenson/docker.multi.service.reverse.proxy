using System;

namespace Response.Formatting
{
    public interface IPathResolver
    {
        string PathOf(string path, Type forType);
    }
}
