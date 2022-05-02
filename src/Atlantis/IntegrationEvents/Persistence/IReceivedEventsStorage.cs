using Atlantis.IntegrationEvents.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Persistence
{
    /// <summary>
    /// Interface para el almacen de eventos recibidos
    /// </summary>
    public interface IReceivedEventsStorage
    {

        /// <summary>
        /// Actualiza el estado de un mensaje recibido
        /// </summary>
        /// <param name="message"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        Task UpdateStatusAsync(MediumMessage message, EventStatus status);

        /// <summary>
        /// Almmacena el error de los eventos recibidos
        /// </summary>
        /// <param name="content"></param>
        void StoreReceivedExceptionMessage(string content);

        /// <summary>
        /// Guarda un mensaje publicado
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        MediumMessage StoreMessage(MediumMessage message);

        // <summary>
        /// Elimina los mensajes expirados
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<int> DeleteExpiresAsync(CancellationToken token = default);

        /// <summary>
        /// Devuelve los mensajes que necesitan ser reintentados
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MediumMessage>> GetReceivedMessagesOfNeedRetry();
    }
}
