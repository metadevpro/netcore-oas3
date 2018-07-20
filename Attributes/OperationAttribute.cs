using System;

namespace Metadev.Oas3.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class OperationAttribute: Attribute
    {
        public string OperationId { get; private set; }

        public OperationAttribute(string operationId)
        {
            OperationId = operationId;
        }
    }
}
