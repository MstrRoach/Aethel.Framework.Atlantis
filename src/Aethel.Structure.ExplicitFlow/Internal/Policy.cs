using Aethel.Extensions.Application.Abstractions.Mediator;
using Aethel.Structure.ExplicitFlow.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Structure.ExplicitFlow.Internal
{
    /// <summary>
    /// Politica a la cual reaccionamos segun un evento especifico
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public class Policy<TEvent> : IPolicy<TEvent>
    {

        /// <summary>
        /// Lista de reacciones con sus instrucciones
        /// de construccion
        /// </summary>
        private List<IReaction<TEvent>> _reactions = new();

        

        /// <summary>
        /// Fabrica para la construccion de la reaccion
        /// </summary>
        //private IReactionFactory _reactionFactory;

        /// <summary>
        /// Define el comando que se enlistara en la reaccion y define
        /// un metodo que construira ese comando reaccion a partir del
        /// evento pasado
        /// </summary>
        /// <typeparam name="TReaction"></typeparam>
        /// <param name="reactionFactory"></param>
        public Policy<TEvent> ThenDo<TReaction>(Func<TEvent,TReaction> reactionFactory)
        {
            // Creamos y configuramos la fabrica para la reaccion\
            var reaction = new Reaction<TEvent, TReaction>(reactionFactory);
            // Agregamos a la lista de reacciones
            _reactions.Add(reaction);
            // Devolvemos la poliza para seguir agregando reacciones
            return this;
        }

        /// <summary>
        /// Devuelve las reacciones convertidas en comandos
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IEnumerable<ICommand> BuildReactions(TEvent @event)
        {
            // Si no hay reacciones devolvemos un vacio
            if (!_reactions.Any())
                yield break;
            // Recorremos las reacciones
            foreach (var reaction in _reactions)
            {
                yield return reaction.Build(@event);
            }
        }

    }
}
