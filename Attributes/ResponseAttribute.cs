using System;

namespace Metadev.Oas3.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class ResponseAttribute: Attribute
    {
        public int StatusCode { get; private set; }
        public string Description { get; private set; }
        public Type ResponseType { get; private set; }

        public ResponseAttribute(int statusCode, string description, Type responseType)
        {
            StatusCode = statusCode;
            Description = description;
            ResponseType = responseType;
        }
    }
}
