namespace UnityEditor.Experimental
{
    public static class NameUtility
    {
        public static string SlugifyConstName(string name)
        {
            return "k" + PascalCase(name);
        }

        public static string SlugifyFieldName(string name)
        {
            return "m_" + PascalCase(name);
        }

        public static string PascalCase(string name)
        {
            if (name.Length == 0)
                return name;

            return name.Substring(0, 1).ToUpperInvariant() + name.Substring(1);
        }
    }
}
