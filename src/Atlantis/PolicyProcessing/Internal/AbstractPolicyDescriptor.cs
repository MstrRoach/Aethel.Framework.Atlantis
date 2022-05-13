using Atlantis.PolicyProcessing.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Internal
{
    /// <summary>
    /// Clase abstracta que contiene lo necesario para definir y utilizar la politica para
    /// un comando dado
    /// </summary>
    public abstract class AbstractPolicyDescriptor<T>
    {

        /// <summary>
        /// Collection de descriptores de politica
        /// </summary>
        internal ConcurrentBag<IPolicyDescriptorInternal<T>> PolicyDescriptors = new();

        /// <summary>
        /// Crea un descriptor de poliza para un evento dado
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public IPolicyBuilder<T, TEvent> PolicyWhen<TEvent>()
            where TEvent : class
        {
            var policy = PolicyDescriptor<T, TEvent>.Create();
            PolicyDescriptors.Add(policy);
            return new PolicyBuilder<T, TEvent>(policy);
        }

        /// <summary>
        /// Se encarga de ejecutar la poliza para el evento pasado por parametro
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <param name="dispatcher"></param>
        public void ExecutePolicy<TEvent>(TEvent @event, IPolicyDispatcher dispatcher)
        {
            // Buscamos el evento correspondiente
            var descriptors = PolicyDescriptors.Where(x => x.IsPolicyFor(@event.GetType())).ToList();
            // ejecutamos el metodo del descriptor para ejecutar el despacho de las reacciones
            descriptors.ForEach(descriptor => descriptor.ExecutePolicy(@event, dispatcher));
        }
    }
}
