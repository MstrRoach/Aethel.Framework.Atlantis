using Aethel.Extensions.Application.Abstractions.Mediator;
using Aethel.Structure.ExplicitFlow.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Structure.ExplicitFlow.Internal
{
    /// <summary>
    /// Clase abstracta que contiene lo necesario para definir y utilizar la politica para
    /// un comando dado
    /// </summary>
    public abstract class AbstractPolicyDescriptor<TCommand>
    {
        /// <summary>
        /// Constructor para
        /// </summary>
        public readonly ProcessDescriptor<TCommand> policyBuilder = new();

        /// <summary>
        /// Devuelve una lista de comandos generados a partir del
        /// evento pasado por parametro
        /// </summary>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="event"></param>
        /// <returns></returns>
        public List<ICommand> GetReactions<TEvent>(TEvent @event)
        {
            // Obtenemos la lista de reacciones asociadas al tipo
            IPolicy<TEvent> policy = policyBuilder.GetPolicyForEvent(@event);
            // Si las polizas estan vacias, salimos
            if (policy is null)
                return new List<ICommand>();
            // Creamos la lista de commandos
            var commands = new List<ICommand>();
            // Recorremos las poliza
            foreach (var command in policy.BuildReactions(@event))
            {
                commands.Add(command);
                Console.WriteLine(command);
            }
            return commands;
        }
    }
}
