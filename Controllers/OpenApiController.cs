using Microsoft.AspNetCore.Mvc;
using Metadev.Oas3.Discover;
using Metadev.Oas3.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Metadev.Oas3.Controllers
{
    [Route("openapi")]
    [Route("openapi.json")]
    public class OpenApiController: Controller
    {
        private DiscoverService Discover;

        public OpenApiController(DiscoverService discover)
        {
            Discover = discover;
        }

        private OpenApiObject Spec;

        [HttpGet]

        public IActionResult Get()
        {
            if (Spec == null) {
                Discover.Build();
                Spec = BuildSpec();
            }
            return new JsonResult(Spec, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,

            });
        }

        private OpenApiObject BuildSpec()
        {
            var spec = new OpenApiObject();
            spec.Info.Title = "alba";
            spec.Info.Version = "1.0.0";
            spec.Paths = Discover.PathsObject;
            spec.Components = Discover.ComponetsObject;
            return spec;
        }
    }
}
