using System;

namespace Metadev.Oas3.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class DescriptionAttribute: Attribute
    {
        public string Description { get; private set; }

        public DescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
