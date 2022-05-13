using Aethel.Extensions.Application.Abstractions.Integration;
using Aethel.Extensions.Application.Jobs;
using Aethel.Extensions.Application.Reflection;
using Atlantis.IntegrationEvents.Internal;
using Atlantis.IntegrationEvents.Persistence;
using Atlantis.PolicyProcessing.Abstractions;
using Atlantis.PolicyProcessing.Internal;
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
        private readonly IPolicyDispatcher _dispatcher;
        private readonly AbstractPolicyDescriptor<IntegrationEvent> _policyDescriptor;
        private readonly TypeManager<IntegrationEvent> _resolver;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="dispatcher"></param>
        /// <param name="policyDescriptor"></param>
        /// <param name="resolver"></param>
        public DispatchingReceivedEvents(IReceivedEventsStorage storage,
            IPolicyDispatcher dispatcher, AbstractPolicyDescriptor<IntegrationEvent> policyDescriptor,
            TypeManager<IntegrationEvent> resolver)
        {
            _storage = storage;
            _dispatcher = dispatcher;
            _policyDescriptor = policyDescriptor;
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
                    _policyDescriptor.ExecutePolicy(@event, _dispatcher);
                    //await _mediator.Publish(@event);
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
