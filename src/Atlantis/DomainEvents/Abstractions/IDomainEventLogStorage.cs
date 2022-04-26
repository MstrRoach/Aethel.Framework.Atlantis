using Atlantis.DomainEvents.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.DomainEvents.Abstractions
{
    /// <summary>
    /// Interface para definir el almacen de los eventos de dominio
    /// procesados. Esta interface permite realizar de forma distinta
    /// la implementacion del storage
    /// </summary>
    public interface IDomainEventLogStorage
    {
        /// <summary>
        /// Agrega un registro de eventos al almacen de registros
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task AddAsyc(DomainEventLog @event);

        /// <summary>
        /// Devuelve todos los eventos no procesados para procesarlos de nuevo
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DomainEventLog>> GetUnprocessedEventsAsync();

        /// <summary>
        /// Actualiza un evento en el almacen de eventos
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        Task UpdateAsync(DomainEventLog @event);
    }
}
