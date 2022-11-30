namespace Api.Domain.Handling.Framework
{
    public class PathResolver : IPathResolver
    {
        public string PathOf(string path, Type forType)
        {
            // generate for type name
            string? prefix = null;
            string? suffix = null;
            var entity = forType.Name.ToLower().Replace("controller", "", StringComparison.InvariantCultureIgnoreCase);

            var index = path.IndexOf(entity);
            if (index > 0)
            {
                prefix = path.Substring(0, index);
                suffix = path.Substring(index + entity.Length, path.Length);
            }

            var result = $"{prefix}{entity}{suffix}";

            return result;
        }
    }
}
