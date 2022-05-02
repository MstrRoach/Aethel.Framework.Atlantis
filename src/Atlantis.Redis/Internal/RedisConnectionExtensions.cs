using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal static class RedisConnectionExtensions
    {
        public static void LogEvents(this IConnectionMultiplexer connection, ILogger logger)
        {
            if (connection is null) throw new ArgumentNullException(nameof(connection));

            if (logger is null) throw new ArgumentNullException(nameof(logger));

            _ = new RedisEvents(connection, logger);
        }
    }
}
