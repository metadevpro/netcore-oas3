using Metadev.Oas3.Discover;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OasServiceCollectionExtensions
    {
        public static void AddOpenApi3(this IServiceCollection serviceCollection, string path)
        {
            serviceCollection.AddSingleton(new DiscoverService(serviceCollection));
            return;
        }
    }
}
