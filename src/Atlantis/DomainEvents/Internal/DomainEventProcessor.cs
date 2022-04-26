using Aethel.Extensions.Application.Reflection;
using Aethel.Extensions.Application.Serialization;
using Aethel.Extensions.Domain;
using Atlantis.DomainEvents.Abstractions;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.DomainEvents.Internal
{
    internal class DomainEventProcessor : IDomainEventProcessor
    {
        /// <summary>
        /// Recollector de eventos 
        /// </summary>
        private readonly IDomainEventCollector _collector;

        /// <summary>
        /// Instancia del mediador para ejecutar los handlers
        /// </summary>
        private readonly IMediator _mediator;

        /// <summary>
        /// Almacen para los registros de eventos despachados
        /// </summary>
        private readonly IDomainEventLogStorage _storage;

        /// <summary>
        /// Contiene la definicion de eventos dond buscar
        /// </summary>
        private readonly TypeManager<IDomainEvent> _resolver;

        /// <summary>
        /// Constructor del procesador de eventos
        /// </summary>
        /// <param name="collector"></param>
        /// <param name="mediator"></param>
        /// <param name="storage"></param>
        /// <param name="resolver"></param>
        public DomainEventProcessor(IDomainEventCollector collector,
            IMediator mediator, IDomainEventLogStorage storage,
            TypeManager<IDomainEvent> resolver)
        {
            _collector = collector;
            _mediator = mediator;
            _storage = storage;
            _resolver = resolver;
        }

        /// <summary>
        /// Distribuye los eventos de dominio y almacena el registro y su estado para 
        /// cada evento ejecutado
        /// </summary>
        /// <param name="rootCommandId"></param>
        /// <returns></returns>
        public async Task DispatchEventsAsync(Guid rootCommandId)
        {
            var events = _collector.GetDomainEvents().ToList();
            _collector.ClearAllDomainEvents();
            
            foreach (var @event in events)
            {
                var type = @event.GetType().FullName;
                var content = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
                {
                    ContractResolver = new AllPropertiesContractResolver()
                });
                var footprint = rootCommandId.ToString();

                var eventLog = new DomainEventLog(footprint, type, content, @event.OccurredOn);

                try
                {
                    await _mediator.Publish(@event);
                    eventLog.WasProcessed();
                }catch (Exception ex)
                {
                    // TODO:MstrRoach Agregar logger
                }
                await _storage.AddAsyc(eventLog);
            }
        }

        /// <summary>
        /// Se encarga de reintentar la publicacion de los eventos de dominio a
        /// traves del mediador
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task RetryDispatchEventsAsync()
        {
            var eventLogs = await _storage.GetUnprocessedEventsAsync();

            foreach (var eventLog in eventLogs)
            {
                var type = _resolver.GetType(eventLog.Type);
                if (type is null)
                {
                    eventLog.WasFailed("Error al recuperar el tipo del evento de dominio");
                    await _storage.UpdateAsync(eventLog);
                    continue;
                }

                var @event = JsonConvert.DeserializeObject(eventLog.Content, type);

                try
                {
                    await _mediator.Publish(@event);
                    eventLog.WasProcessed();
                }catch (Exception ex)
                {
                    // TODO:MstrRoach Agregar logger
                }
                await _storage.UpdateAsync(eventLog);
            }
        }

    }
}
