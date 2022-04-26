using Aethel.Extensions.Domain;
using Atlantis.DomainEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.DomainEvents.Internal
{
    /// <summary>
    /// Centraliza los eventos de domminio a lo lartgo de todos
    /// los repositorios para distribuirlos antes de confirmar
    /// la transaccion
    /// </summary>
    internal class DomainEventCollector : IDomainEventCollector
    {
        /// <summary>
        /// Lista de eventos de dominio que estan libres para
        /// ser despachados
        /// </summary>
        private List<IDomainEvent> _events = new List<IDomainEvent>();

        /// <summary>
        /// Recibe un aggregate root que contiene los eventos generados por
        /// un agregado y los extrae, para despues limpiar el agregado de los
        /// eventos que contenia
        /// </summary>
        /// <param name="root"></param>
        public void ExtractEvents(IAggregateRoot root)
        {
            this._events.AddRange(root.DomainEvents);
            root.ClearDomainEvents();
        }

        /// <summary>
        /// Devuelve una representacion de solo lectura de los eventos de dominio
        /// recolectados
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IDomainEvent> GetDomainEvents() => _events.AsReadOnly();

        /// <summary>
        /// Elimmmina todos los eventos de dominio existentes en el collector
        /// </summary>
        public void ClearAllDomainEvents() => this._events.Clear();


    }
}
