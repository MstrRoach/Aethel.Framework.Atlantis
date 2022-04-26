using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.DomainEvents.Abstractions
{
    /// <summary>
    /// Se encarga de distribuir los eventos de dominio
    /// utrilizando la instancia de mediatr para efectuar
    /// los cambios en el modulo de forma inmediata
    /// </summary>
    public interface IDomainEventProcessor
    {

        /// <summary>
        /// Despacha todos los eventos de dominio asociados al comando que 
        /// se pasa por parametro
        /// </summary>
        /// <returns></returns>
        Task DispatchEventsAsync(Guid rootCommandId);

        /// <summary>
        /// Realiza los intentos por redepachar los eventos que no se
        /// procesaron aun
        /// </summary>
        /// <returns></returns>
        Task RetryDispatchEventsAsync();
    }
}
