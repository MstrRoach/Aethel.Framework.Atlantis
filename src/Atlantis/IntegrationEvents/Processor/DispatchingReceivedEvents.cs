using Aethel.Extensions.Application.Abstractions.Integration;
using Aethel.Extensions.Application.Jobs;
using Aethel.Extensions.Application.Reflection;
using Atlantis.IntegrationEvents.Internal;
using Atlantis.IntegrationEvents.Persistence;
using Atlantis.PolicyProcessing.Abstractions;
using MediatR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Processor
{
    /// <summary>
    /// Procesa los eventos de integracion recibidos
    /// </summary>
    public class DispatchingReceivedEvents : IProcessingAction
    {
        private readonly IReceivedEventsStorage _storage;
        private readonly IMediator _mediator;
        private readonly TypeManager<IntegrationEvent> _resolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="mediator"></param>
        /// <param name="resolver"></param>
        public DispatchingReceivedEvents(IReceivedEventsStorage storage, ICommandPolicyProcessor<IntegrationEvent> policyProcessor,
            IMediator mediator, TypeManager<IntegrationEvent> resolver)
        {
            _mediator = mediator;
            _storage = storage;
            _resolver = resolver;
        }

        /// <summary>
        /// Distribuye los eventos de integracion en el modulo
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            var messages = await _storage.GetReceivedMessagesOfNeedRetry();

            foreach (var message in messages)
            {
                var type = _resolver.GetType(message.Type);
                if (type is null)
                {
                    message.Error = "Error al recuperar el tipo del evento";
                    message.IncraseRetry();
                    await _storage.UpdateStatusAsync(message, EventStatus.Failed);
                    continue;
                }

                var @event = JsonConvert.DeserializeObject(message.Content, type);

                try
                {
                    await _mediator.Publish(@event);
                    message.ProcessedAt = DateTime.UtcNow;
                    await _storage.UpdateStatusAsync(message, EventStatus.Succeeded);
                }
                catch(Exception ex)
                {
                    // Loggeammmos algo
                    message.Error = ex.Message;
                    message.Retries++;
                    await _storage.UpdateStatusAsync(message, EventStatus.Failed);
                }
                
            }
        }
    }
}
