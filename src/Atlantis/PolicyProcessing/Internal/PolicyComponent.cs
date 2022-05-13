using Atlantis.PolicyProcessing.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Internal
{
    /// <summary>
    /// Encapsula la reaccion a partir del evento dado
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <typeparam name="TReaction"></typeparam>
    public class PolicyComponent<TEvent, TReaction> : IPolicyComponent<TEvent>
    {
        /// <summary>
        /// Expresion utilizada para la construccion de la reaccion
        /// </summary>
        private Func<TEvent, TReaction> reactionFactory;

        /// <summary>
        /// Constructor de la reaccion
        /// </summary>
        /// <param name="reactionFactory"></param>
        public PolicyComponent(Expression<Func<TEvent, TReaction>> reactionFactory)
        {
            this.reactionFactory = reactionFactory.Compile();
        }

        /// <summary>
        /// Construye y despacha los eventos utilizando la instancia de despacho pasada por parametro
        /// </summary>
        /// <param name="event"></param>
        /// <param name="dispatcher"></param>
        /// <exception cref="NotImplementedException"></exception>
        public async Task DispatchAsync(TEvent @event, IPolicyDispatcher dispatcher)
        {
            // Compilamos la reaccion
            var reaction = reactionFactory(@event);
            // Despachammos a traves del despachador
            await dispatcher.Dispatch<TReaction>(reaction);
        }
    }
}
