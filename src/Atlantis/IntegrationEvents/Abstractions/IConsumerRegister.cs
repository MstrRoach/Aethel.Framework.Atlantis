using Atlantis.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Abstractions
{
    /// <summary>
    /// Se encarga de realizar el consumo de los eventos
    /// </summary>
    public interface IConsumerRegister : IProcessingServer
    {
        /// <summary>
        /// Verifica si el proceso esta vivoi
        /// </summary>
        /// <returns></returns>
        bool IsHealthy();

        /// <summary>
        /// Reinicia el procesador
        /// </summary>
        /// <param name="force"></param>
        void ReStart(bool force = false);
    }
}
