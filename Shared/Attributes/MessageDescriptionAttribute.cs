using System.ComponentModel;

namespace Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MessageDescription : DescriptionAttribute
    {
        public MessageDescription(string description) : base(description)
        {
        }
    }
}

