using Atlantis.IntegrationEvents.Internal;
using Atlantis.IntegrationEvents.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Transport
{
    /// <summary>
    /// Contiene la logica para enviar a traves de algun bus de eventos
    /// </summary>
    public interface ITransport
    {
        /// <summary>
        /// Envia el mensaje al bus de eventos
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<OperateResult> SendAsync(MediumMessage message);
    }
}
