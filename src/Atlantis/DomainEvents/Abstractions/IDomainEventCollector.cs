using Aethel.Extensions.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.DomainEvents.Abstractions
{
    /// <summary>
    /// Recolecta todos los eventos de dominio explicitamente
    /// </summary>
    public interface IDomainEventCollector
    {

        /// <summary>
        /// Devuelve todos los eventos de dominio que existen dentro
        /// del collector
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDomainEvent> GetDomainEvents();

        /// <summary>
        /// Extrae los eventos de una raiz agregada y los almacena
        /// dentro del collector
        /// </summary>
        /// <param name="root"></param>
        void ExtractEvents(IAggregateRoot root);

        /// <summary>
        /// Limpia todos los eventos del colector
        /// </summary>
        void ClearAllDomainEvents();
    }
}
