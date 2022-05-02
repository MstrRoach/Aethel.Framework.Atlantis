using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Abstractions
{
    /// <summary>
    /// Pool de conexiones para redis
    /// </summary>
    internal interface IRedisConnectionPool
    {
        /// <summary>
        /// Devuelve una conexion del pool de conexiones
        /// </summary>
        /// <returns></returns>
        Task<IConnectionMultiplexer> ConnectAsync();
    }
}
