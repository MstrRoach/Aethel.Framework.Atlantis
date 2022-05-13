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
    /// Constructor de la politica
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public class PolicyBuilder<TCommand, TEvent> : IPolicyBuilder<TCommand, TEvent>
    {
        /// <summary>
        /// Descriptor de la politica
        /// </summary>
        public IPolicyDescriptor<TCommand, TEvent> Descriptor { get; }

        public PolicyBuilder(IPolicyDescriptor<TCommand, TEvent> descriptor)
        {
            Descriptor = descriptor;
        }

        /// <summary>
        /// Agrega una nueva reaccion a la politica actual
        /// </summary>
        /// <typeparam name="TReaction"></typeparam>
        /// <param name="policyMapper"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IPolicyBuilder<TCommand, TEvent> ThenDo<TReaction>(Expression<Func<TEvent, TReaction>> reactionFactory)
        {
            // Agregamos al descriptor la fabrica de reaccion configurada
            Descriptor.AddReactionFactory(reactionFactory);
            return this;
        }
    }
}
