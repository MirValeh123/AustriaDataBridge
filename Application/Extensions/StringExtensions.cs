namespace Application.Extensions
{
    public static class StringExtensions
    {
        public static string ReplaceWordsWithObjectProperties(string template, object data)
        {
            if (string.IsNullOrEmpty(template) || data == null)
                return template;

            var properties = data.GetType().GetProperties();

            foreach (var prop in properties)
            {
                string name = prop.Name;
                string value = prop.GetValue(data)?.ToString() ?? string.Empty;

                // Case-sensitive əvəzetmə
                template = template.Replace(name, value);
            }

            return template;
        }
    }
}
