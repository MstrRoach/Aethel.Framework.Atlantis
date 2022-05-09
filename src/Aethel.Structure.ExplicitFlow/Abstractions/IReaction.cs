using Aethel.Extensions.Application.Abstractions.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Structure.ExplicitFlow.Abstractions
{
    public interface IReaction<TEvent>
    {
        /// <summary>
        /// Construye un comando a partir de la funcion almacenada
        /// en la reaccion
        /// </summary>
        /// <returns></returns>
        ICommand Build(TEvent @event);
    }
}
