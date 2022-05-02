using Aethel.Extensions.Application.Abstractions.Integration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Abstractions
{
    /// <summary>
    /// Interface de publicacion para todos los eventos que
    /// necesiten ser despachados
    /// </summary>
    public interface IPublisher
    {

        /// <summary>
        /// Publicador de mensajes asincronio
        /// </summary>
        /// <typeparam name="T">Objeto contenedor</typeparam>
        /// <param name="event">Contenido del evento a enviar</param>
        /// <param name="cancellationToken">Token de cancelacion para las operaciones</param>
        /// <returns></returns>
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent;
    }
}
