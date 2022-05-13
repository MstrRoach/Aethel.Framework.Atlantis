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
    /// Define el descriptor para las politicas para cada
    /// evento desencadenante
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TEvent"></typeparam>
    public class PolicyDescriptor<TCommand, TEvent> :
        IPolicyDescriptor<TCommand, TEvent>,
        IPolicyDescriptorInternal<TCommand> where TEvent : class
    {
        /// <summary>
        /// Tipo del evento que desencadena la politica
        /// </summary>
        public Type TriggerEvent { get; }

        /// <summary>
        /// Lista de componentes configurados
        /// </summary>
        private readonly List<IPolicyComponent<TEvent>> _components = new();

        /// <summary>
        /// Fabrica estatica para el descriptor de poliza
        /// </summary>
        /// <returns></returns>
        public static PolicyDescriptor<TCommand, TEvent> Create()
        {
            // Creamos el descriptor inicial
            return new PolicyDescriptor<TCommand, TEvent>(typeof(TEvent));
        }

        /// <summary>
        /// Agrega una fabrica de reaccion para construir los comandos de reaccion
        /// </summary>
        /// <typeparam name="TReaction"></typeparam>
        /// <param name="reactionFactory"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void AddReactionFactory<TReaction>(Expression<Func<TEvent, TReaction>> reactionFactory)
        {
            // Creamos el componente 
            var component = new PolicyComponent<TEvent, TReaction>(reactionFactory);
            // Agregamos el componente a la lista
            _components.Add(component);
        }

        /// <summary>
        /// Permite identificar si el evento pasado por parametro generico
        /// es el mismo que el evento desencadenante
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool IsPolicyFor<T>()
        {
            var type = typeof(T);
            return TriggerEvent == type;
        }

        /// <summary>
        /// Permite identificar si el evento pasado por parametro generico
        /// es el mismo que el evento desencadenante
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        public bool IsPolicyFor(Type eventType) => TriggerEvent == eventType;

        /// <summary>
        /// Despacha los comandos a traves de suis componentes
        /// </summary>
        /// <typeparam name="TEvent1"></typeparam>
        /// <param name="event"></param>
        /// <param name="dispatcher"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void ExecutePolicy(object @event, IPolicyDispatcher dispatcher)
        {
            var wrapperEvent = @event as TEvent;
            _components.ForEach(x => x.DispatchAsync(wrapperEvent, dispatcher));
        }

        

        /// <summary>
        /// Constructor de descriptor de poliza
        /// </summary>
        /// <param name="triggerEvent"></param>
        public PolicyDescriptor(Type triggerEvent)
        {
            TriggerEvent = triggerEvent;
        }

    }
}
