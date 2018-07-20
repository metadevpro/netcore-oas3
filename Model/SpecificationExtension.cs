using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Metadev.Oas3.Model
{

    public interface SpecificationExtension
    {
       IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class ExtensionValue
    {
        [DataMember]
        public string Value { get; set; }
        [DataMember]
        public string Type { get; set; }
    }
}
