using Aethel.Redis.Abstractions;
using Aethel.Redis.Internal;
using Atlantis.IntegrationEvents.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System.Net;

namespace Aethel.Redis
{
    public static class AethelRedisExtensions
    {
        /// <summary>
        /// Agrega redis
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IServiceCollection AddRedis(this IServiceCollection services, Action<RedisOptions> configure)
        {
            services.AddSingleton<IRedisConnectionPool, RedisConnectionPool>();
            services.AddSingleton<IRedisStreamManager, RedisStreamManager>();
            services.AddSingleton<IConsumerClientFactory, RedisConsumerClientFactory>();
            services.AddSingleton<ITransport, RedisTransport>();
            services.TryAddEnumerable(ServiceDescriptor
               .Singleton<IPostConfigureOptions<RedisOptions>, CapRedisOptionsPostConfigure>());
            services.AddOptions<RedisOptions>().Configure(configure);
            return services;
        }


    }

    /// <summary>
    /// Configuracion despues de agregar la configuracion inicial
    /// </summary>
    internal class CapRedisOptionsPostConfigure : IPostConfigureOptions<RedisOptions>
    {
        public void PostConfigure(string name, RedisOptions options)
        {
            options.Configuration ??= new ConfigurationOptions();

            if (options.StreamEntriesCount == default)
                options.StreamEntriesCount = 10;

            if (options.ConnectionPoolSize == default)
                options.ConnectionPoolSize = 10;

            if (!options.Configuration.EndPoints.Any())
            {
                options.Configuration.EndPoints.Add(IPAddress.Loopback, 0);
                options.Configuration.SetDefaultPorts();
            }
        }
    }
}