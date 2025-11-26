namespace Shared.Helpers
{
    public static class TextFileReader
    {
        public static async Task<string> ReadAsStringAsync(string path)
        {
            using var reader = new StreamReader(path);
            var content = await reader.ReadToEndAsync();
            reader.Close();

            return content;
        }

        public static async Task WriteAsStringAsync(string path, string text)
        {
            char[] chars = text.ToCharArray();

            using var reader = new StreamWriter(path);
            await reader.WriteLineAsync(chars, 0, chars.Length);
            reader.Close();
        }
    }
}
