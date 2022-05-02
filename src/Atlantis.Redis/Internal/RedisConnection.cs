using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    /// <summary>
    /// Contiene los metodos para deolver una conexion valida hacia redis
    /// </summary>
    public class RedisConnection : IDisposable
    {
        private bool _isDisposed;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RedisConnection(IConnectionMultiplexer connection)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <summary>
        /// Acceso a la conexion 
        /// </summary>
        public IConnectionMultiplexer Connection { get; }

        /// <summary>
        /// Propiedad para saber el resument de la capacidad de conexion
        /// </summary>
        public long ConnectionCapacity => Connection.GetCounters().TotalOutstanding;

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;

            if (disposing) Connection.Dispose();

            _isDisposed = true;
        }
    }
}
