using System.ComponentModel;
using System.Reflection;
using Shared.Attributes;

namespace Shared.Helpers
{
    public static class EnumHelper
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
        public static int ToInt(this Enum value) => Convert.ToInt32(value);
        public static string GetEnumExtraDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(ExtraDescriptionAttribute), false) as ExtraDescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
        public static string GetEnumMessageDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            MessageDescription[] attributes = fi.GetCustomAttributes(typeof(MessageDescription), false) as MessageDescription[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }

}
