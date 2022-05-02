using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis
{
    public class RedisOptions
    {

        /// <summary>
        /// Configuracion nativa de StackExchange.Redis
        /// </summary>
        public ConfigurationOptions? Configuration { get; set; }

        /// <summary>
        /// Direccion a la que nos conectamos
        /// </summary>
        public string Endpoint { get; set; } = default;

        /// <summary>
        /// Indica cual es el tamaño de la picina de conexiones
        /// </summary>
        public int ConnectionPoolSize { get; set; } = 10;

        /// <summary>
        /// Indica cuantas entradas vamos a leer
        /// </summary>
        public int StreamEntriesCount { get; set; } = 10;

        

    }
}
