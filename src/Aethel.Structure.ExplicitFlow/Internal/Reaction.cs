using Aethel.Extensions.Application.Abstractions.Mediator;
using Aethel.Structure.ExplicitFlow.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Structure.ExplicitFlow.Internal
{
    public class Reaction<TEvent, TReaction> : IReaction<TEvent>
    {
        /// <summary>
        /// Funcion anonima para la construccion dela reaccion esperada
        /// </summary>
        private Func<TEvent, TReaction> _functionFactory;

        /// <summary>
        /// Constructor para la fabrica de reacciones
        /// </summary>
        /// <param name="functionFactory"></param>
        internal Reaction(Func<TEvent, TReaction> functionFactory)
        {
            _functionFactory = functionFactory;
        }

        /// <summary>
        /// Construye la funcion a partir del evento pasado por parametro
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public ICommand Build(TEvent @event)
        {
            return _functionFactory(@event) as ICommand;
        }
    }
}
