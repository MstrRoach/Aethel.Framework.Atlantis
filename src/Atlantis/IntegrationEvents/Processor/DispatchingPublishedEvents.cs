using Aethel.Extensions.Application.Jobs;
using Atlantis.IntegrationEvents.Abstractions;
using Atlantis.IntegrationEvents.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Processor
{
    /// <summary>
    /// Proceso para despachar los eventos de integracion publicados con error
    /// </summary>
    public class DispatchingPublishedEvents : IProcessingAction
    {
        private readonly IPublishedEventsStorage _storage;
        private readonly IMessageSender _messageSender;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="sender"></param>
        public DispatchingPublishedEvents(IPublishedEventsStorage storage,
            IMessageSender sender)
        {
            _storage = storage;
            _messageSender = sender;
        }

        /// <summary>
        /// Ejecutamos la logica
        /// </summary>
        /// <returns></returns>
        public async Task Execute()
        {
            var messages = await _storage.GetPublishedMessagesOfNeedRetry();

            foreach (var message in messages)
            {
                await _messageSender.SendAsync(message);
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

    }
}
