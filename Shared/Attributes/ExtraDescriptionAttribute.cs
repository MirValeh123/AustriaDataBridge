using System.ComponentModel;

namespace Shared.Attributes
{
    public class ExtraDescriptionAttribute : DescriptionAttribute
    {
        public ExtraDescriptionAttribute(string description) : base(description)
        {
        }

        public ExtraDescriptionAttribute(int description) : this(description.ToString())
        {
        }
    }
}
