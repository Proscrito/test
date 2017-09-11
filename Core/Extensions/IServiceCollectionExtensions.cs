using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using yamvc.Core.WebSocket;

namespace yamvc.Core.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigOptions<TOptions>(this IServiceCollection services,
            IConfiguration configuration, string section) where TOptions : class, new()
        {
            services.Configure<TOptions>(configuration.GetSection(section));
            services.AddSingleton(cfg => cfg.GetService<IOptions<TOptions>>().Value);
            return services;
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            services.AddTransient<WebSocketConnectionManager>();

            foreach (var type in Assembly.GetEntryAssembly().ExportedTypes)
            {
                if (type.GetTypeInfo().BaseType == typeof(WebSocketHandler))
                {
                    services.AddSingleton(type);
                }
            }

            return services;
        }
    }
}
