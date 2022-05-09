using Aethel.Extensions.Application.Abstractions.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Structure.ExplicitFlow.Abstractions
{
    public interface IPolicy<TEvent> : IPolicy
    {
        /// <summary>
        /// Devuelve las reacciones construidas a partir de la funcion de 
        /// construccion asociada a cada evento
        /// </summary>
        /// <returns></returns>
        IEnumerable<ICommand> BuildReactions(TEvent @event);
    }

    /// <summary>
    /// Interfaz para el agrupamiento de las polizas
    /// </summary>
    public interface IPolicy { }
}
