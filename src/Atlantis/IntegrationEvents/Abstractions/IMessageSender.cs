using Atlantis.IntegrationEvents.Internal;
using Atlantis.IntegrationEvents.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Abstractions
{
    /// <summary>
    /// Enviador de los mensajes a traves de la capa de transporte
    /// </summary>
    public interface IMessageSender
    {
        /// <summary>
        /// Envia el mensaje a traves del enviador
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<OperateResult> SendAsync(MediumMessage message);
    }
}
