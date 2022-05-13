using Aethel.Extensions.Application.Serialization;
using Aethel.Extensions.Domain;
using Atlantis.DomainEvents.Abstractions;
using Atlantis.DomainEvents.Internal;
using Atlantis.InternalCommands.Abstractions;
using Atlantis.PolicyProcessing.Internal;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Permite recolectar los eventos de dominios y utilizar el procesador
    /// de polizas para guardarlas
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CommandPolicyProcessor<T> : ICommandPolicyProcessor<T>
    {
        /// <summary>
        /// Recolector de los eventos de dominio
        /// </summary>
        private readonly IDomainEventCollector _collector;

        /// <summary>
        /// Contiene las reglas de operacion para eventos dados y originados a
        /// partir del comando
        /// </summary>
        private readonly AbstractPolicyDescriptor<T>? _policyDescriptor;

        /// <summary>
        /// Agendador de los comandos internos
        /// </summary>
        private readonly IPolicyDispatcher _scheduler;

        /// <summary>
        /// Objeto de logeo
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Almacen para los registros de eventos despachados
        /// </summary>
        private readonly IDomainEventLogStorage _storage;

        /// <summary>
        /// Constructor para el procesador de politicas de comando
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="policyDescriptor"></param>
        /// <param name="scheduler"></param>
        /// <param name="logger"></param>
        /// <param name="storage"></param>
        public CommandPolicyProcessor(IDomainEventCollector collector,
            IPolicyDispatcher scheduler, ILogger<CommandPolicyProcessor<T>> logger, 
            IDomainEventLogStorage storage, AbstractPolicyDescriptor<T>? policyDescriptor = null)
        {
            _collector = collector;
            _policyDescriptor = policyDescriptor;
            _scheduler = scheduler;
            _logger = logger;
            _storage = storage;
        }

        /// <summary>
        /// Ejecuta la poliza para un comando dado
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        public async Task ExecutePolicy(Guid sourceId)
        {
            // Recollectamos los eventos
            var events = _collector.GetDomainEvents()
                .ToList();
            _collector.ClearAllDomainEvents();

            // Procesamos los eventos
            await ApplyPolicies(sourceId, events);
        }

        /// <summary>
        /// Permite aplicar la poliza para el commando actual
        /// </summary>
        /// <param name="sourceCommand"></param>
        /// <param name="events"></param>
        /// <returns></returns>
        private async Task ApplyPolicies(Guid sourceCommand, List<IDomainEvent> events)
        {
            if (_policyDescriptor is null)
                return;
            // Lista de reaccion globales
            var allReactions = new List<IReaction>();
            // Recorremos los eventos
            foreach (var @event in events)
            {
                // Ejecutamos las polizas por cada evento
                _policyDescriptor.ExecutePolicy(@event,_scheduler);                
                // Serializamos el contenido del evento
                var content = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    ContractResolver = new AllPropertiesContractResolver()
                });
                // Creammos el evento
                var domainEvent = new DomainEventLog(sourceCommand.ToString()
                    , @event.GetType().FullName,
                    content,
                    @event.OccurredOn);
                // Marcamos como procesado
                domainEvent.WasProcessed();
                // Guardamos
                await _storage.AddAsyc(domainEvent);
            }
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
            foreach (var reaction in reactions)
            {
                information = $"{information} | {Environment.NewLine} ------ {reaction.GetType().Name} * Applied {Environment.NewLine}";
            }
            _logger.LogInformation(information);
        }
    }

    
}
