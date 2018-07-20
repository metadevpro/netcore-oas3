using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Metadev.Oas3.Model;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Routing;
using Metadev.Oas3.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Security;

namespace Metadev.Oas3.Discover
{
    public class DiscoverService
    {
        private IServiceCollection Services;
        public PathsObject PathsObject;
        public ComponetsObject ComponetsObject;

        private IDictionary<string, Type> DefineSchemasFor = new Dictionary<string, Type>();

        public DiscoverService(IServiceCollection services)
        {
            Services = services;
        }
        public void Build()
        {
            PathsObject = new PathsObject();

            foreach (ServiceDescriptor service in Services)
            {
                if (IsControllerBase(service.ServiceType))
                {
                    var ty = service.ServiceType;
                    var controllerAtrs = ty.GetCustomAttributes();
                    var methods = ty.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                    foreach (MethodInfo methInf in methods)
                    {
                        var methodAtrs = methInf.GetCustomAttributes();
                        if (IsOperation(methInf, methodAtrs))
                        {
                            var pathSegment = BuildPathSegment(ty, controllerAtrs, methodAtrs);
                            var verb = GetVerb(methodAtrs);
                            var operationObj  = BuildOperationObject(controllerAtrs, methInf, methodAtrs);
                            if (operationObj != null)
                            {
                                PathItemObject pi;
                                if (PathsObject.ContainsKey(pathSegment))
                                {
                                    pi = PathsObject[pathSegment];
                                }
                                else
                                {
                                    pi = new PathItemObject();
                                    PathsObject[pathSegment] = pi;
                                }
                                InsertOPerationInPathItem(pi, verb, operationObj);
                            }
                        }
                    }
                }
            }
            if (DefineSchemasFor.Keys.Count > 0)
            {
                ComponetsObject = new ComponetsObject();
                ComponetsObject.Schemas = new Dictionary<string, SchemaObject>();
                foreach (string key in DefineSchemasFor.Keys)
                {
                    ComponetsObject.Schemas[key] = DeriveSchemaForType(DefineSchemasFor[key]);
                }
            }
        }

     
        private string GetVerb(IEnumerable<Attribute> methodAtrs)
        {
            if (methodAtrs.Any(a => a.GetType() == typeof(HttpGetAttribute)))
            {
                return "get";
            }
            if (methodAtrs.Any(a => a.GetType() == typeof(HttpPostAttribute)))
            {
                return "post";
            }
            if (methodAtrs.Any(a => a.GetType() == typeof(HttpPutAttribute)))
            {
                return "put";
            }
            if (methodAtrs.Any(a => a.GetType() == typeof(HttpDeleteAttribute))) 
            {
                return "delete";
            }
            if (methodAtrs.Any(a => a.GetType() == typeof(HttpHeadAttribute)))
            {
                return "head";
            }
            if (methodAtrs.Any(a => a.GetType() == typeof(HttpOptionsAttribute)))
            {
                return "options";
            }
            if (methodAtrs.Any(a => a.GetType() == typeof(HttpPatchAttribute)))
            {
                return "patch";
            }
            return null;
        }

        private void InsertOPerationInPathItem(PathItemObject pi, string verb, OperationObject operationObj)
        {
            if (verb == "get")
            {
                pi.Get = operationObj;
            }
            else if (verb == "post")
            {
                pi.Post = operationObj;
            }
            else if (verb == "put")
            {
                pi.Put = operationObj;
            }
            else if (verb == "delete")
            {
                pi.Delete = operationObj;
            }
            else if (verb == "head")
            {
                pi.Head = operationObj;
            }
            else if (verb == "options")
            {
                pi.Options = operationObj;
            }
            else if (verb == "patch")
            {
                pi.Patch = operationObj;
            }
        }

        private bool IsOperation(MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            var find = methodAtrs.Any(a => a.GetType().IsSubclassOf(typeof(HttpMethodAttribute)));
            return (find);
        }

        private bool IsControllerBase(Type type)
        {
            if (type != null)
            {
                if (type.IsSubclassOf(typeof(ControllerBase)))
                {
                    return true;
                }
            }
            return false;
        }

        private string BuildPathSegment(Type controller, 
                                        IEnumerable<Attribute> controllerAtrs, 
                                        IEnumerable<Attribute> methodAtrs)
        {
            var basePath = "";
            var atrRoute = controllerAtrs.FirstOrDefault(atr => atr.GetType() == typeof(RouteAttribute)) as RouteAttribute;
            if (atrRoute != null)
            {
                basePath = atrRoute.Template.Replace("[controller]", DeriveName(controller.Name) );
            }
            var subpath = methodAtrs.FirstOrDefault(a => a.GetType().IsSubclassOf(typeof(HttpMethodAttribute))) as HttpMethodAttribute;
            if (subpath != null && subpath.Template != null)
            {
                basePath += "/" + subpath.Template;
            }
            if (!basePath.StartsWith('/'))
            {
                basePath = "/" + basePath;
            }
            return basePath;
        }

        private string DeriveName(string name)
        {
            var i = name.LastIndexOf("Controller");
            if (i >= 0)
            {
                var res = name.Substring(0, i);
                
                return ToCamelCase(res);
            }
            return ToCamelCase(name);
        }
        private string ToCamelCase(string name)
        {
            var res = name;
            var ch = res[0];
            if (ch >= 'A' && ch <= 'Z')
            {
                var ch2 = ch.ToString().ToLowerInvariant();
                res = ch2 + res.Substring(1);
            }
            return res;
        }

        private OperationObject BuildOperationObject(IEnumerable<Attribute> controllerAtrs, 
                                             MethodInfo methInf,
                                             IEnumerable<Attribute> methodAtrs)
        {
            var item = new OperationObject();
            item.OperationId = DeriveOperationId(methInf, methodAtrs);
            item.Description = DeriveDescription(methodAtrs);
            item.Summary = DeriveSummary(methodAtrs);
            item.Responses = new ResponsesObject();
            var tags = BuildTags(controllerAtrs, methInf, methodAtrs);
            if (tags != null)
            {
                item.Tags = tags;
            }
            var parameters = BuildParameters(controllerAtrs, methInf, methodAtrs);
            if (parameters != null)
            {
                item.Parameters = parameters;
            }
            var requestBody = BuildRequestBody(controllerAtrs, methInf, methodAtrs);
            if (requestBody != null)
            {
                item.RequestBody = requestBody;
            }
            item.Responses = BuildResponses(controllerAtrs, methInf, methodAtrs);

            return item;
        }

        private RequestBodyObjectOrReference BuildRequestBody(IEnumerable<Attribute> controllerAtrs, MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            var bodyParam = methInf.GetParameters().FirstOrDefault(p =>
            {
                var atrs = p.GetCustomAttributes().FirstOrDefault(a => 
                    a.GetType() == typeof(FromBodyAttribute) ||
                    a.GetType() == typeof(FromFormAttribute)
                    );
                return atrs != null;
            });
            if (bodyParam != null)
            {
                var res = new RequestBodyObject();
                res.Content = new Dictionary<string, MediaTypeObject>();
                res.Content["application/json"] = BuildMediaTypeObject(methInf.ReturnType);                
                return  res;
            }
            return null;
        }

        private ResponsesObject BuildResponses(IEnumerable<Attribute> controllerAtrs, MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            var res = new ResponsesObject();

            res.Default = BuildDefaultResponse(controllerAtrs, methInf, methodAtrs);
            var extraResponses = BuildAdditionalResponses(controllerAtrs, methInf, methodAtrs);
            if (extraResponses.Keys.Count > 0)
            {
                res.Responses = extraResponses;
            }
            return res;            
        }

        private ResponseObjectOrReference BuildDefaultResponse(IEnumerable<Attribute> controllerAtrs, MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            var res = new ResponseObject();
            var atr = methodAtrs.FirstOrDefault(a => a.GetType() == typeof(ResponseAttribute)) as ResponseAttribute;
            if (atr != null)
            {
                res.Description = atr.Description;
                res.Content = new Dictionary<string, MediaTypeObject>();
                res.Content["application/json"] = BuildMediaTypeObject(atr.ResponseType);
            }
            else
            {
                res.Description = "Default response";
                res.Content = new Dictionary<string, MediaTypeObject>();
                res.Content["application/json"] = BuildDefaultMediaTypeObject(controllerAtrs, methInf, methodAtrs);
            }
            return res;
        }

        private MediaTypeObject BuildMediaTypeObject(Type type)
        {
            var res = new MediaTypeObject();
            res.Schema = BuildSchemaForType(type);
            return res;
        }
        private MediaTypeObject BuildDefaultMediaTypeObject(IEnumerable<Attribute> controllerAtrs, MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            return BuildMediaTypeObject(methInf.ReturnType);
        }

        private IDictionary<int, ResponseObjectOrReference> BuildAdditionalResponses(
                IEnumerable<Attribute> controllerAtrs, 
                MethodInfo methInf, 
                IEnumerable<Attribute> methodAtrs)
        {
            var res = new Dictionary<int, ResponseObjectOrReference>();
            var atrs = methodAtrs.Where(a => a.GetType() == typeof(ResponseAttribute))
                                .Skip(1)
                                .ToList() as IList<ResponseAttribute>;
            if (atrs != null)
            {
                foreach (ResponseAttribute ra in atrs)
                {
                    res[ra.StatusCode] = BuildResponseObject(ra);
                }
            }

            return res;
        }

        private ResponseObjectOrReference BuildResponseObject(ResponseAttribute ra)
        {
            var ro = new ResponseObject();
            ro.Description = ra.Description;
            ro.Content = new Dictionary<string, MediaTypeObject>();
            ro.Content["application/json"] = BuildMediaTypeObject(ra.ResponseType);
            return ro;
        }

        private IEnumerable<ParameterObjectOrReference> BuildParameters(IEnumerable<Attribute> controllerAtrs, MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            var result = new List<ParameterObjectOrReference>();
            foreach (ParameterInfo par in methInf.GetParameters())
            {
                var parameterAtrs = par.GetCustomAttributes();

                var par2 = new ParameterObject();
                par2.Name = par.Name;
                var inValue = DeriveIn(methodAtrs, parameterAtrs);
                if (inValue == "body")
                {
                    continue; // skip body params, add them as Request Body Object.
                }
                par2.In = inValue;
                par2.Required = IsRequired(par, parameterAtrs);
                var sc = BuildSchemaForType(par.ParameterType);
                par2.Schema = sc;

                result.Add(par2);
            }
            if (result.Count == 0)
            {
                return null;
            }
            return result;
        }

     

        private string DeriveIn(IEnumerable<Attribute> methodAtrs, IEnumerable<Attribute> parameterAtrs)
        {
            var bodyAtr = parameterAtrs.FirstOrDefault(a => a.GetType() == typeof(FromBodyAttribute));
            if (bodyAtr != null)
            {
                return "body";
            }
            var formAtr = parameterAtrs.FirstOrDefault(a => a.GetType() == typeof(FromFormAttribute));
            if (formAtr != null)
            {
                return "body";
            }
            var h = parameterAtrs.FirstOrDefault(a => a.GetType() == typeof(FromHeaderAttribute));
            if (h!= null)
            {
                return "header";
            }
            var q = parameterAtrs.FirstOrDefault(a => a.GetType() == typeof(FromQueryAttribute));
            if (q != null)
            {
                return "query";
            }
            var r = parameterAtrs.FirstOrDefault(a => a.GetType() == typeof(FromRouteAttribute));
            if (r != null)
            {
                return "path";
            }
            var s = parameterAtrs.FirstOrDefault(a => a.GetType() == typeof(FromServicesAttribute));
            if (s != null)
            {
                return "path";  // undef
            }
            return "path";
        }

        private IEnumerable<string> BuildTags(IEnumerable<Attribute> controllerAtrs, MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            var result = new List<string>();
            var atrs1 = controllerAtrs.Where(a => a.GetType() == typeof(TagAttribute)).ToList();
            var atrs2 = methodAtrs.Where(a => a.GetType() == typeof(TagAttribute)).ToList();
            var atrs3 = new List<Attribute>();
            if (atrs1 != null)
            {
                atrs3.AddRange(atrs1);
            }
            if (atrs2 != null)
            {
                atrs3.AddRange(atrs2);
            }
            foreach (TagAttribute tag in atrs3)
            {
                result.Add(tag.Tag);
            }
            if (result.Count == 0)
            {
                return null;
            }
            return result;
        }

        private string DeriveDescription(IEnumerable<Attribute> methodAtrs)
        {
            var atr = methodAtrs.FirstOrDefault(a => a.GetType() == typeof(DescriptionAttribute)) as DescriptionAttribute;
            if (atr != null)
            {
                return atr.Description;
            }
            return null;
        }
        private string DeriveSummary(IEnumerable<Attribute> methodAtrs)
        {
            var atr = methodAtrs.FirstOrDefault(a => a.GetType() == typeof(SummaryAttribute)) as SummaryAttribute;
            if (atr != null)
            {
                return atr.Summary;
            }
            return null;
        }

        private string DeriveOperationId(MethodInfo methInf, IEnumerable<Attribute> methodAtrs)
        {
            var atr = methodAtrs.FirstOrDefault(a => a.GetType() == typeof(OperationAttribute)) as OperationAttribute;
            if (atr != null)
            {
                return atr.OperationId;
            }
            return methInf.Name;
        }
        private SchemaObjectOrReference BuildSchemaForType(Type type)
        {
            if (type == typeof(void))
            {
                return null;
            }
            if (type == typeof(IActionResult))
            {
                var s1 = new SchemaObject();
                s1.Type = "object";
                s1.Description = "No enought information provided to constrain the type.";
                return s1;
            }

            var primitiveType = GetPrimitiveType(type);
            if (IsEnumerable(type))
            {
                var itemType = GetItemTypeInArray(type);
                var sArray = new SchemaObject();
                sArray.Type = "array";
                sArray.Items = BuildSchemaForType(itemType);
                return sArray;
            }
            else if (primitiveType != null)
            {
                return primitiveType;
            }
            else
            {
                var sr = new SchemaReference();
                sr.Ref = BuildOpenApiReference(type.Name);

                // enroll for later type-description
                DefineSchemasFor[type.Name] = type;
                return sr;
            }
        }

        private Type GetItemTypeInArray(Type type)
        {
            var isEnumerable = type.GetInterfaces()
                                   .Any(inter => inter.Name == "IEnumerable");
            if (isEnumerable)
            {
                if (type.IsConstructedGenericType)
                {
                    var gt = type.GetGenericArguments().FirstOrDefault();
                    return gt;
                }
                else
                {
                    return null;
                }
            }
            else if (type.IsArray)
            {
                var it = type.ReflectedType;
                return it;
            }
            return null;  // not found
        }

        private bool IsEnumerable(Type type)
        {
            if (type.IsPrimitive || type == typeof(String))
            {
                return false;
            }
            var isEnumerable = type.GetInterfaces()
                                   .Any(inter => inter.Name == "IEnumerable");

            if (isEnumerable || 
                type.IsSubclassOf(typeof(IEnumerable<Object>)) || 
                type.IsArray)
            {
                return true;
            }
            return false;
        }

        private string BuildOpenApiReference(string name)
        {
            return "#/components/schemas/" + ToCamelCase(name);
        }

        private SchemaObject GetPrimitiveType(Type type)
        {
            var sc = new SchemaObject();
            if (type == typeof(int))
            {
                sc.Type = "integer";
                sc.Format = "int32";
                return sc;
            }
            if (type == typeof(long))
            {
                sc.Type = "integer";
                sc.Format = "int64";
                return sc;
            }
            if (type == typeof(float))
            {
                sc.Type = "number";
                sc.Format = "float";
                return sc;
            }
            if (type == typeof(double))
            {
                sc.Type = "number";
                sc.Format = "double";
                return sc;
            }
            if (type == typeof(Decimal))
            {
                sc.Type = "number";
                sc.Format = "double";
                return sc;
            }
            if (type == typeof(string))
            {
                sc.Type = "string";
                return sc;
            }
            if (type == typeof(Byte[]))
            {
                sc.Type = "string";
                sc.Format = "byte";
                return sc;
            }
            if (type == typeof(bool))
            {
                sc.Type = "boolean";
                return sc;
            }
            if (type == typeof(DateTime))
            {
                // todo: add heuristic to disambiguate
                sc.Type = "string";
                sc.Format = "date-time";
                return sc;
            }
            if (type == typeof(DateTime))
            {
                // todo: add heuristic to disambiguate
                sc.Type = "string";
                sc.Format = "date";
                return sc;
            }
           
            if (type == typeof(SecureString))
            {
                sc.Type = "string";
                sc.Format = "password";
                return sc;
            }
            return null;
        }

        private bool IsRequired(ParameterInfo par, IEnumerable<Attribute> parameterAtrs)
        {
            var r = parameterAtrs.FirstOrDefault(a => a.GetType() == typeof(RequiredAttribute));
            if (r != null)
            {
                return true;
            }
            if (par.IsOptional)
            {
                return false;
            }
            return true;
        }
        private bool IsRequiredProperty(PropertyInfo prop)
        {
            var atrs = prop.GetCustomAttributes();
            var r = atrs.FirstOrDefault(a => a.GetType() == typeof(RequiredAttribute));
            if (r != null)
            {
                return true;
            }
            return false;
        }
        private SchemaObject DeriveSchemaForType(Type type)
        {
            var sc = new SchemaObject();
            sc.Type = "object";
            sc.Properties = new Dictionary<string, SchemaObjectOrReference>();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                sc.Properties[prop.Name] = BuildSchemaForType(prop.PropertyType);
            }
            var required = new List<string>();
            foreach (PropertyInfo prop in type.GetProperties())
            {
                if (IsRequiredProperty(prop))
                {
                    required.Add(ToCamelCase(prop.Name));
                }
            }
            if (required.Count > 0)
            {
                sc.Required = required;
            }
            return sc;
        }

      
    }
}
