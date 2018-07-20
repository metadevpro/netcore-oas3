using System;

namespace Metadev.Oas3.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SummaryAttribute: Attribute
    {
        public string Summary { get; private set; }

        public SummaryAttribute(string summary)
        {
            Summary = summary;
        }
    }
}
