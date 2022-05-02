using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Transport
{
    /// <summary>
    /// Fabrica de clientes de consumidor
    /// </summary>
    public interface IConsumerClientFactory
    {
        /// <summary>
        /// Crea una nueva instancia del consummidor especificado
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        IConsumerClient Create(string groupId);
    }
}
