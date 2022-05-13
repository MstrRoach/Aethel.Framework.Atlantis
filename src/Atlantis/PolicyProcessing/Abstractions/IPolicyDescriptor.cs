using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Define el comportamiento especificado para cada comando con su
    /// evento
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public interface IPolicyDescriptor<TCommand, TEvent>
    {
        /// <summary>
        /// Agrega una fabrica de reaccion al descriptor de poliza para cuando se necesiten
        /// despachar los comandos
        /// </summary>
        /// <typeparam name="TReaction"></typeparam>
        /// <param name="reactionFactory"></param>
        void AddReactionFactory<TReaction>(Expression<Func<TEvent, TReaction>> reactionFactory);
    }
}
