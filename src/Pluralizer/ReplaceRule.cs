using System.Text.RegularExpressions;

namespace Pluralizer
{
    public class ReplaceRule
    {
        public Regex Condition { get; set; }
        public string ReplaceWith { get; set; }
    }
}
