using System;

namespace Response.Formater
{
    public interface IPathResolver
    {
        string PathOf(string path, Type forType);
    }
}
