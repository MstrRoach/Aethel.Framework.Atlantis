using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Interface para el constructor de politica
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public interface IPolicyBuilder<TCommand, TEvent>
    {
        /// <summary>
        /// Agrega un nuevo comando de reaccion y agrega el mappeo esperado para convertir entre
        /// el tipo de evento y el tipo de comando
        /// </summary>
        /// <typeparam name="TReaction"></typeparam>
        /// <param name="policyMapper"></param>
        /// <returns></returns>
        IPolicyBuilder<TCommand, TEvent> ThenDo<TReaction>(Expression<Func<TEvent, TReaction>> policyMapper);
    }
}
