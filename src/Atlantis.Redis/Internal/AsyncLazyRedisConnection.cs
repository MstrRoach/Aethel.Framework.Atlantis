using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal class AsyncLazyRedisConnection : Lazy<Task<RedisConnection>>
    {
        /// <summary>
        /// Constructor de la conexion perezosa
        /// </summary>
        /// <param name="redisOptions"></param>
        /// <param name="logger"></param>
        public AsyncLazyRedisConnection(RedisOptions redisOptions,
            ILogger<AsyncLazyRedisConnection> logger) : base(() => ConnectAsync(redisOptions, logger))
        {
        }

        public TaskAwaiter<RedisConnection> GetAwaiter()
        {
            return Value.GetAwaiter();
        }

        /// <summary>
        /// Constrctor de la conexion
        /// </summary>
        /// <param name="redisOptions"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static async Task<RedisConnection> ConnectAsync(RedisOptions redisOptions,
            ILogger<AsyncLazyRedisConnection> logger)
        {
            int attemp = 1;

            var redisLogger = new RedisLogger(logger);

            ConnectionMultiplexer? connection = null;

            while (attemp <= 5)
            {
                connection = await ConnectionMultiplexer.ConnectAsync(redisOptions.Configuration, redisLogger)
                .ConfigureAwait(false);

                connection.LogEvents(logger);

                if (!connection.IsConnected)
                {
                    logger.LogWarning($"Can't establish redis connection,trying to establish connection [attemp {attemp}].");
                    await Task.Delay(TimeSpan.FromSeconds(2));
                    ++attemp;
                }
                else
                    attemp = 6;
            }
            if (connection == null)
                throw new Exception($"Can't establish redis connection,after [{attemp}] attemps.");

            return new RedisConnection(connection);
        }
    }
}
