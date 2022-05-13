using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Define el comportamiento de la poliza solo por comandos
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public interface IPolicyDescriptorInternal<TCommand>
    {
        /// <summary>
        /// Recibe el evento y realiza el despacho de los mismos a traves del
        /// descriptor
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <param name="dispatcher"></param>
        void ExecutePolicy(object @event, IPolicyDispatcher dispatcher);

        /// <summary>
        /// Indica si el evento pasado por parametro generico
        /// corresponde al tipo de evento que detona la politica
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        bool IsPolicyFor<T>();

        /// <summary>
        /// Indica si el tipo del evento coincide con algun descriptor
        /// de poliza que detona las reacciones
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        bool IsPolicyFor(Type eventType);
    }
}
