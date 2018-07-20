using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Metadev.Oas3.Model
{
    [DataContract]
    public class ReferenceObject
    {
        [DataMember(Name = "$ref")]
        [Required]
        public string Ref { get; set; }
    }
    public interface CallbackObjectOrReference {}
    public interface ResponseObjectOrReference {}
    public interface RequestBodyObjectOrReference {}
    public interface ExampleObjectOrReference {}
    public interface SchemaObjectOrReference {}
    public interface ParameterObjectOrReference {}
    public interface LinkObjectOrReference {}

    [DataContract]
    public class LinkReference : ReferenceObject, LinkObjectOrReference
    {}
    public class ParameterReference : ReferenceObject, ParameterObjectOrReference
    {}
    [DataContract]
    public class CallbackReference : ReferenceObject, CallbackObjectOrReference
    {}
    [DataContract]
    public class RequestBodyReference : ReferenceObject, RequestBodyObjectOrReference
    {}
    [DataContract]
    public class ExampleReference : ReferenceObject, ExampleObjectOrReference
    {}
    [DataContract]
    public class SchemaReference : ReferenceObject, SchemaObjectOrReference
    {}
}
