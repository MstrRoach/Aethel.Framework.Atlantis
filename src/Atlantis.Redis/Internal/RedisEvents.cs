using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal class RedisEvents
    {
        private readonly ILogger _logger;

        public RedisEvents(IConnectionMultiplexer connection, ILogger logger)
        {
            this._logger = logger;
            connection.ErrorMessage += Connection_ErrorMessage;
            connection.ConnectionRestored += Connection_ConnectionRestored;
            connection.ConnectionFailed += Connection_ConnectionFailed;
        }

        private void Connection_ConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogError(e.Exception,
                $"Connection failed!, {e.Exception?.Message}, for endpoint:{e.EndPoint}, failure type:{e.FailureType}, connection type:{e.ConnectionType}");
        }

        private void Connection_ConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            _logger.LogWarning(
                $"Connection restored back!, {e.Exception?.Message}, for endpoint:{e.EndPoint}, failure type:{e.FailureType}, connection type:{e.ConnectionType}");
        }

        private void Connection_ErrorMessage(object sender, RedisErrorEventArgs e)
        {
            _logger.LogError($"Server replied with error, {e.Message}, for endpoint:{e.EndPoint}");
        }
    }
}
