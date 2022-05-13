using Aethel.Extensions.Domain;
using Atlantis.InternalCommands.Abstractions;
using Atlantis.PolicyProcessing.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Internal
{
    /// <summary>
    /// Sirve para ejecutar las polizas segun lo especificado en cada una de ellas
    /// </summary>
    public class PolicyProcessor<T>
    {
        /// <summary>
        /// Contiene las reglas de operacion para eventos dados y originados a
        /// partir del comando
        /// </summary>
        private readonly AbstractPolicyDescriptor<T> _policyDescriptor;

        /// <summary>
        /// Agendador de los comandos internos
        /// </summary>
        private readonly IInternalCommandScheduler _scheduler;

        /// <summary>
        /// Objeto de logeo
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor del procesador de polizas
        /// </summary>
        /// <param name="policyDescriptor"></param>
        /// <param name="scheduler"></param>
        /// <param name="logger"></param>
        public PolicyProcessor(AbstractPolicyDescriptor<T> policyDescriptor, 
            IInternalCommandScheduler scheduler, ILogger<PolicyProcessor<T>> logger)
        {
            _policyDescriptor = policyDescriptor;
            _scheduler = scheduler;
            _logger = logger;
        }

        /// <summary>
        /// Permite aplicar la poliza para el commando actual
        /// </summary>
        /// <param name="sourceCommand"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        public async Task ApplyPolicies(Guid sourceCommand, List<IDomainEvent> events)
        {
            if (_policyDescriptor is null)
                return;
            // Lista de reaccion globales
            var allReactions = new List<IReaction>();
            // Recorremos los eventos
            foreach (var @event in events)
            {
               
            }
            await Task.CompletedTask;
        } 

        /// <summary>
        /// Logea en orden de arbol la politica aplicada
        /// </summary>
        /// <param name="event"></param>
        /// <param name="reactions"></param>
        private void LogReactionsTree(INotification @event, List<IReaction> reactions)
        {
            var information = "Policy applied";
            information = $"{information} {Environment.NewLine}";
            information = $"{information} --- {@event.GetType().Name} {Environment.NewLine}";
            foreach(var reaction in reactions)
            {
                information = $"{information} | {Environment.NewLine} ------ {reaction.GetType().Name} * Applied {Environment.NewLine}";
            }
            _logger.LogInformation(information);
        }
    }
}
