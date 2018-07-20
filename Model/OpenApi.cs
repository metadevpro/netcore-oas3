using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Metadev.Oas3.Model
{
    [DataContract]
    public class OpenApiObject: SpecificationExtension
    {
        [DataMember]
        public string Openapi { get; set; }
        [DataMember]
        public InfoObject Info { get; set; }
        [DataMember]
        public IEnumerable<ServerObject> Servers { get; set; }
        [DataMember]
        public PathsObject Paths { get; set; }
        [DataMember]
        public ComponetsObject Components { get; set; }
        [DataMember]
        public SecurityRequirementObject Security { get; set; }
        [DataMember]
        public TagObject Tags { get; set; }
        [DataMember]
        public ExternalDocumentationObject ExternalDocs { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }

        public OpenApiObject()
        {
            Openapi = "3.0.1";
            Info = new InfoObject();
            // Servers = new List<ServerObject>();
            Paths = new PathsObject();
            // Components = new ComponetsObject();
            // Security = new SecurityRequirementObject();
            // Tags = new TagObject();
            // ExternalDocs = new ExternalDocumentationObject();
            // Extensions = new Dictionary<string, ExtensionValue>();
        }
    }

    [DataContract]
    public class InfoObject: SpecificationExtension
    {
        [Required]
        [DataMember]
        public string Title { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string TermsOfService { get; set; }
        [DataMember]
        public string Version { get; set; }
        [DataMember]
        public ContactObject Contact { get; set; }
        [DataMember]
        public LicenseObject License { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class ContactObject: SpecificationExtension
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Email { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class LicenseObject: SpecificationExtension
    {
        [Required]
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class ServerObject: SpecificationExtension
    {
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public IDictionary<string, ServerVariableObject> Variables { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class ServerVariableObject: SpecificationExtension
    {
        [DataMember]
        public IEnumerable<string> Enum { get; set; }  // any primitive
        [DataMember]
        public string Default { get; set; }         // object -> primitive
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class PathsObject: Dictionary<string, PathItemObject>, SpecificationExtension
    {     
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }

        public PathsObject()
        {
            // Extensions = new Dictionary<string, ExtensionValue>();
        }
    }
    [DataContract]
    public class PathItemObject : SpecificationExtension
    {
        [DataMember(Name = "$ref")]
        public string Ref { get; set; }         // $ref
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public OperationObject Get { get; set; }
        [DataMember]
        public OperationObject Post { get; set; }
        [DataMember]
        public OperationObject Put { get; set; }
        [DataMember]
        public OperationObject Delete { get; set; }
        [DataMember]
        public OperationObject Options { get; set; }
        [DataMember]
        public OperationObject Head { get; set; }
        [DataMember]
        public OperationObject Patch { get; set; }
        [DataMember]
        public OperationObject Trace { get; set; }
        [DataMember]
        public IEnumerable<ServerObject> Servers { get; set; }
        [DataMember]
        public IEnumerable<ParameterObjectOrReference> Parameters { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }

        public PathItemObject()
        {
            // Servers = new List<ServerObject>();
            // Parameters = new List<ParameterObject>();
            // Extensions = new Dictionary<string, ExtensionValue>();
        }
    }



    /* Common base for ParamObject and HeaderObject */
    [DataContract]
    public class ParamHeaderBase 
    {     
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public bool Required { get; set; }
        [DataMember]
        public bool Deprecated { get; set; }
        [DataMember]
        public bool AllowEmptyValue { get; set; }
        [DataMember]
        public string Style { get; set; }
        [DataMember]
        public bool Explode { get; set; }
        [DataMember]
        public bool AllowReserved { get; set; }
        [DataMember]
        public SchemaObjectOrReference Schema { get; set; }
        [DataMember]
        public IEnumerable<ExampleObjectOrReference> Examples { get; set; }
        [DataMember]
        public ExampleObjectOrReference Example { get; set; }
        [DataMember]
        public IDictionary<string, MediaTypeObject> Content { get; set; }
    }

    [DataContract]
    public class ParameterObject: ParamHeaderBase, ParameterObjectOrReference, SpecificationExtension
    {
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        [Required]
        public string In { get; set; }

        // properties inherited from ParamHeaderBase    
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class SchemaObject: SchemaObjectOrReference, SpecificationExtension
    {
        [DataMember]
        public string Type { get; set; }
        [DataMember]
        public SchemaObjectOrReference AllOf { get; set; }
        [DataMember]
        public SchemaObjectOrReference MultipleOf { get; set; }
        [DataMember]
        public SchemaObjectOrReference OneOf { get; set; }
        [DataMember]
        public SchemaObjectOrReference AnyOf { get; set; }
        [DataMember]
        public SchemaObjectOrReference Not { get; set; }
        [DataMember]
        public SchemaObjectOrReference Items { get; set; }
        [DataMember]
        public IDictionary<string, SchemaObjectOrReference> Properties { get; set; }
        [DataMember]
        public IDictionary<string, SchemaObjectOrReference> AdditionalProperties { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public string Format { get; set; }
        [DataMember]
        public object Default { get; set; }
        [DataMember]
        public IEnumerable<string >Enum { get; set; }
        [DataMember]
        public IEnumerable<string> Required{ get; set; }
        [DataMember]
        public int MaxProperties { get; set; }
        [DataMember]
        public int MinProperties { get; set; }
        [DataMember]
        public int UniqueItems { get; set; }
        [DataMember]
        public int MinItems { get; set; }
        [DataMember]
        public int MaxItems { get; set; }
        [DataMember]
        public string Pattern { get; set; }
        [DataMember]
        public int MinLenght { get; set; }
        [DataMember]
        public int MaxLenght { get; set; }
        [DataMember]
        public int ExclusiveMinimum { get; set; }
        [DataMember]
        public int Minimum { get; set; }
        [DataMember]
        public int ExclusiveMaximum { get; set; }
        [DataMember]
        public int Maximum { get; set; }
        [DataMember]
        public bool Nullable { get; set; }
        [DataMember]
        public string Discriminator { get; set; }
        [DataMember]
        public bool Readonly { get; set; }
        [DataMember]
        public bool WriteOnly { get; set; }
        [DataMember]
        public XmlObject Xml { get; set; }
        [DataMember]
        public ExternalDocumentationObject ExternalDocs { get; set; }
        [DataMember]
        public IEnumerable<ExampleObjectOrReference> Examples { get; set; }
        [DataMember]
        public ExampleObjectOrReference Example { get; set; }
        [DataMember]
        public bool Deprecated { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class XmlObject: SpecificationExtension
    {
        [DataMember]
        public string Name{ get; set; }
        [DataMember]
        public string Namespace { get; set; }
        [DataMember]
        public string Prefix { get; set; }
        [DataMember]
        public bool Attribute { get; set; }
        [DataMember]
        public bool Wrapped { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class ExampleObject : ExampleObjectOrReference
    {
        [DataMember]
        public string Example { get; set; }
    }

    [DataContract]
    public class MediaTypeObject: SpecificationExtension
    {
        [DataMember]
        public SchemaObjectOrReference Schema { get; set; }
        [DataMember]
        public IEnumerable<ExampleObjectOrReference> Examples { get; set; }
        [DataMember]
        public ExampleObjectOrReference Example{ get; set; }
        [DataMember]
        public EncodingObject Encoding { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class EncodingObject : SpecificationExtension
    {
        [DataMember]
        public IDictionary<string, EncodingPropertyObject> Properties { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class EncodingPropertyObject: SpecificationExtension
    {

        [DataMember]
        public string ContentType { get; set; }
        [DataMember]
        public IDictionary<string, string> Headers { get; set; }
        [DataMember]
        public string Style { get; set; }
        [DataMember]
        public bool Explode { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class OperationObject : SpecificationExtension
    {
        [DataMember]
        public IEnumerable<string> Tags { get; set; }
        [DataMember]
        public string Summary { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public ExternalDocumentationObject externalDocs { get; set; }
        [DataMember]
        public string OperationId { get; set; }
        [DataMember]
        public IEnumerable<ParameterObjectOrReference> Parameters { get; set; }
        [DataMember]
        public RequestBodyObjectOrReference RequestBody{ get; set; }
        [DataMember]
        public ResponsesObject Responses { get; set; }
        [DataMember]
        public CallbacksObject Callbacks { get; set; }
        [DataMember]
        public bool Deprecated { get; set; }
        [DataMember]
        public SecurityRequirementObject Security { get; set; }
        [DataMember]
        public IEnumerable<ServerObject>  Servers { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class RequestBodyObject: RequestBodyObjectOrReference, SpecificationExtension
    {
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public IDictionary<string, MediaTypeObject> Content { get; set; }
        [DataMember]
        public bool Required { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }

    [DataContract]
    public class ResponsesObject: SpecificationExtension
    {
        [DataMember]
        public ResponseObjectOrReference Default { get; set; }
        [DataMember]
        public IDictionary<int, ResponseObjectOrReference> Responses { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }



    [DataContract]
    public class ResponseObject: ResponseObjectOrReference, SpecificationExtension
{
        [Required]
        [DataMember()]
        public string Description { get; set; }
        [DataMember()]
        public HeaderObject Headers { get; set; }
        [DataMember()]
        public IDictionary<string, MediaTypeObject> Content { get; set; }
        [DataMember()]
        public LinksObject Links{ get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class ResponseReference: ReferenceObject, ResponseObjectOrReference
    {  }

  

    [DataContract]
    public class CallbacksObject: SpecificationExtension
    {
        [DataMember]
        public IDictionary<string, CallbackObjectOrReference> Callbacks {get; set;}
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }


    [DataContract]
    public class ComponetsObject: SpecificationExtension
    {
        [DataMember]
        public IDictionary<string, SchemaObject> Schemas { get; set; }
        [DataMember]
        public IDictionary<string, ResponseObject> Responses { get; set; }
        [DataMember]
        public IDictionary<string, ParameterObject> Parameters { get; set; }
        [DataMember]
        public IDictionary<string, ExampleObject> Examples { get; set; }
        [DataMember]
        public IDictionary<string, RequestBodyObject> RequestBodies { get; set; }
        [DataMember]
        public IDictionary<string, HeaderObject> Header { get; set; }
        [DataMember]
        public IDictionary<string, SecuritySchemeObject> SecuritySchemes { get; set; }
        [DataMember]
        public IDictionary<string, LinkObject> Links { get; set; }
        [DataMember]
        public IDictionary<string, CallbackObject> Callbacks { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class HeadersObject
    {
        [DataMember]
        public IDictionary<string, HeaderObjectOrReference> Headers { get; set; }
    }
    public interface HeaderObjectOrReference { }
    [DataContract]
    public class HeaderObject: ParamHeaderBase, HeaderObjectOrReference
    {
        // inherited from ParamHeaderBase
    }
    [DataContract]
    public class HeaderReference : ReferenceObject, HeaderObjectOrReference
    {
    }
    [DataContract]
    public class SecuritySchemeObject: SpecificationExtension
    {
        [DataMember]
        [Required]
        public string Type { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        [Required]
        public string Name { get; set; }
        [DataMember]
        [Required]
        public string In { get; set; }
        [DataMember]
        [Required]
        public bool Required { get; set; }
        [DataMember]
        [Required]
        public string Scheme { get; set; }
        [DataMember]
        public string BearerFormat { get; set; }
        [DataMember]
        public OAuthFlowObject Flow { get; set; }
        [DataMember]
        public string OpenIdConnectUrl { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class OAuthFlowObject: SpecificationExtension
    {
        [DataMember]
        [Required]
        public string AuthorizationUrl { get; set; }
        [DataMember]
        [Required]
        public string TokenUrl { get; set; }
        [DataMember]
        public string RefreshUrl { get; set; }
        [DataMember]
        [Required]
        public ScopesObject Scopes { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class ScopesObject: SpecificationExtension
    {
        public IDictionary<string, string> Scopes { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class LinksObject
    {
        [DataMember]
        public IDictionary<string, LinkObjectOrReference> Links { get; set; }
    }

    [DataContract]
    public class LinkObject: LinkObjectOrReference, SpecificationExtension
{
        [DataMember]
        public string Href { get; set; }
        [DataMember]
        public string OperationId { get; set; }
        [DataMember]
        public IEnumerable<LinkParameterObject> Parameters { get; set; }
        [DataMember]
        public HeadersObject Headers { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class LinkParameterObject
    {
        [DataMember]
        public string Name{ get; set; }
        [DataMember]
        public object Expression { get; set; }
    }
    [DataContract]
    public class SecurityRequirementObject
    {
        public IDictionary<string, IEnumerable<string>> Requirements { get; set; }
    }

    [DataContract]
    public class CallbackObject: CallbackObjectOrReference, SpecificationExtension
    {
        [DataMember]
        IDictionary<string, PathItemObject> CallbackItems { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class TagObject: SpecificationExtension
    {
        [Required]
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public ExternalDocumentationObject ExternalDocs { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
    [DataContract]
    public class ExternalDocumentationObject : SpecificationExtension
    {
        [DataMember]
        public string Description { get; set; }
        [Required]
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public IDictionary<string, ExtensionValue> Extensions { get; set; }
    }
}
