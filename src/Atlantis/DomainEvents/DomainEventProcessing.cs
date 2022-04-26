using Aethel.Extensions.Application.Jobs;
using Atlantis.DomainEvents.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.DomainEvents
{
    public class DomainEventProcessing : IProcessingAction
    {
        private readonly IDomainEventProcessor _eventProcessor;
        public DomainEventProcessing(IDomainEventProcessor eventProcessor)
        {
            _eventProcessor = eventProcessor;
        }

        public async Task Execute()
        {
            await _eventProcessor.RetryDispatchEventsAsync();
        }
    }
}
