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
    /// Objeto para describir y almacenar las polizas por eventos.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    public class ProcessDescriptor<TCommand>
    {
        /// <summary>
        /// Diccionario con las reacciones asociados a los eventos generados
        /// por la ejecucion del comando
        /// </summary>
        private ConcurrentDictionary<string, IPolicy> policies = new();

        /// <summary>
        /// Obtiene la poliza asociada a un eventos
        /// </summary>
        /// <param name="eventType"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        internal IPolicy<TEvent> GetPolicyForEvent<TEvent>(TEvent @event)
        {
            var eventName = typeof(TEvent);
            if(policies.TryGetValue(eventName.Name, out var policy))
                return policy as IPolicy<TEvent>;
            return null;
        }

        /// <summary>
        /// Inicia una configuracion para reacciones
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <returns></returns>
        public Policy<TEvent> When<TEvent>()
        {
            // Obtenemos el tipo del evento
            var eventType = typeof(TEvent);
            // Creamos una nueva reaccion
            var policy = new Policy<TEvent>();
            // La agregamos a la lista concurrente
            policies[eventType.Name] = policy;
            // Retornamos la reaccion para ser procesada
            return policy;
        }



    }
}
