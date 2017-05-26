using Microsoft.Extensions.DependencyInjection;

namespace Net.Chdk.Providers.Boot
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBootProviderResolver(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IBootProviderResolver, BootProviderResolver>();
        }

        public static IServiceCollection AddScriptProvider(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddSingleton<IScriptProvider, ScriptProvider>();
        }
    }
}
