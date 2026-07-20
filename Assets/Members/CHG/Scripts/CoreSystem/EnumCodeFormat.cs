namespace Members.CHG.Scripts.CoreSystem
{
    public static class EnumCodeFormat
    {
        public static string EnumFormat = 
@"
namespace {0}
{{
    public enum {1}
    {{
        {2}
    }}
}}
";
    }
}