namespace Domain.Handling
{
    public class PathResolver : IPathResolver
    {
        public string PathOf(string path, Type forType)
        {
            // generate for type name
            string result = null;
            string prefix = null;
            string suffix = null;
            string entity = forType.Name.ToLower().Replace("controller", "", StringComparison.InvariantCultureIgnoreCase);

            var index = path.IndexOf(entity);
            if (index > 0)
            {
                prefix = path.Substring(0, index);
                suffix = path.Substring(index + entity.Length, path.Length);
            }

            result = $"{prefix}{entity}{suffix}";

            return result;
        }
    }
}
