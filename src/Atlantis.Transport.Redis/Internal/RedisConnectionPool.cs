using Aethel.Redis.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal class RedisConnectionPool : IRedisConnectionPool, IDisposable
    {
        private readonly ConcurrentBag<AsyncLazyRedisConnection> _connections = new();

        private readonly ILoggerFactory _loggerFactory;
        private readonly RedisOptions _redisOptions;
        private readonly SemaphoreSlim _poolLock = new(1);
        private bool _isDisposed;
        private bool _poolAlreadyConfigured;

        public RedisConnectionPool(IOptions<RedisOptions> options, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _redisOptions = options.Value;
            Init().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Propiedad que indica si el pool ya fue configurado
        /// </summary>
        private AsyncLazyRedisConnection? QuietConnection
        {
            get
            {
                return _poolAlreadyConfigured ? _connections.OrderBy(async c => (await c).ConnectionCapacity).First() : null;
            }
        }

        /// <summary>
        /// Inicia las conexiones
        /// </summary>
        /// <returns></returns>
        private async Task Init()
        {
            try
            {
                await _poolLock.WaitAsync();

                if (_connections.Any())
                    return;

                for (var i = 0; i < _redisOptions.ConnectionPoolSize; i++)
                {
                    var connection = new AsyncLazyRedisConnection(_redisOptions,
                        _loggerFactory.CreateLogger<AsyncLazyRedisConnection>());

                    _connections.Add(connection);
                }
            }
            finally
            {
                _poolLock.Release();
            }
        }


        /// <summary>
        /// Realiza la conexion devolviendola del pull de conexiones
        /// </summary>
        /// <returns></returns>
        public async Task<IConnectionMultiplexer> ConnectAsync()
        {
            if (QuietConnection == null)
            {
                _poolAlreadyConfigured = _connections.Count(c => c.IsValueCreated) == _redisOptions.ConnectionPoolSize;
                if (QuietConnection != null)
                    return (await QuietConnection).Connection;
            }

            foreach (var lazy in _connections)
            {
                if (!lazy.IsValueCreated)
                    return (await lazy).Connection;

                var connection = await lazy;
                if (connection.ConnectionCapacity == default)
                    return connection.Connection;
            }

            return (await _connections.OrderBy(async c => (await c).ConnectionCapacity).First()).Connection;
        }

        /// <summary>
        /// Libera recursos admministrados
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Liberacion de recursos administrada
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing)
                foreach (var connection in _connections)
                {
                    if (!connection.IsValueCreated)
                        continue;

                    connection.GetAwaiter().GetResult().Dispose();
                }

            _isDisposed = true;
        }
    }
}
