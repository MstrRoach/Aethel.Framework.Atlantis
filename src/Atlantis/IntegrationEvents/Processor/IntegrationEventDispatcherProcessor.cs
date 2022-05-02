using Atlantis.Abstractions;
using Atlantis.IntegrationEvents.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Processor
{
    internal class IntegrationEventDispatcherProcessor : IProcessor
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _provider;
        public IntegrationEventDispatcherProcessor(ILogger<IntegrationEventDispatcherProcessor> logger,
            IServiceProvider provider)
        {
            _logger = logger;
            _provider = provider;
        }


        public async Task ProcessAsync(ProcessingContext context)
        {
            _logger.LogInformation(
                "Inicio de procesamiento para distribucion de eventos de integracion" + Environment.NewLine +
                "==================================================================="
                );
            await context.WaitAsync(TimeSpan.FromSeconds(60));
        }

        /// <summary>
        /// Despacha los eventos de integracion recibidos
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task ProcessReceivedEvents(ProcessingContext context)
        {
            var dispatching = context.CreateScope()
                .Provider
                .GetRequiredService<DispatchingReceivedEvents>();
            await dispatching.Execute();
        }


        private async Task ProcessPublishedEvents()
        {

            await Task.CompletedTask;
        }
    }
}
