using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Abstractions
{
    /// <summary>
    /// Un subproceso del proceso de mensajeria
    /// </summary>
    public interface IProcessingServer : IDisposable
    {
        /// <summary>
        /// Verifica si el proceso esta vivo
        /// </summary>
        void Pulse() { }

        /// <summary>
        /// Inicia el procesamiento
        /// </summary>
        /// <param name="stoppingToken"></param>
        void Start(CancellationToken stoppingToken);
    }
}
