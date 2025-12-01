using System.Xml.Serialization;

namespace Shared.Helpers
{
    public static class XmlConverter
    {
        public static string ConvertToXml<T>(T source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return Serialize(source);
        }

        private static string Serialize<T>(T obj)
        {
            using (var stringWriter = new StringWriter())
            {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringWriter, obj);
                return stringWriter.ToString();
            }
        }
    }
}
