using System;

namespace Metadev.Oas3.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class TagAttribute: Attribute
    {
        public string Tag { get; private set; }

        public TagAttribute(string tag)
        {
            Tag = tag;
        }
    }
}
