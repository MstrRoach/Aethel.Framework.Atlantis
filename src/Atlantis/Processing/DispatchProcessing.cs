using Atlantis.Abstractions;
using Atlantis.IntegrationEvents.Processor;
using Atlantis.InternalCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Processing
{
    /// <summary>
    /// Proceso para el despacho de eventos de dominio en reintentos, comandos internos y 
    /// eventos de integracion de salida y de entrada
    /// </summary>
    public class DispatchProcessing : IProcessor
    {
        private readonly ILogger _logger;
        private readonly TimeSpan _delay = TimeSpan.FromSeconds(1);
        private readonly TimeSpan _waitingInterval = TimeSpan.FromSeconds(60);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        public DispatchProcessing(ILogger<DispatchProcessing> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ProcessAsync(ProcessingContext context)
        {
            _logger.LogInformation(
                "# ----------- Inicio de procesamiento para distribucion de eventos de Atlantis ----------- #");

            await Task.WhenAll(
                ProcessReceivedIntegrationEvents(context),
                ProcessPublishedIntegrationEvents(context),
                ProcessInternalCommands(context)
                );
            _logger.LogInformation(
                "# ----------- Finalizando el procesamiento de eventos de Atlantis ----------- #");
            await context.WaitAsync(_waitingInterval);
        }

        /// <summary>
        /// Ejecuta el procesammiento de los eventos de integracion recibidos
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task ProcessReceivedIntegrationEvents(ProcessingContext context)
        {
            _logger.LogInformation("============= Procesando los eventos de integracion recibidos ==========");
            context.ThrowIfStopping();
            var dispatch = context
                .CreateScope()
                .Provider
                .GetRequiredService<DispatchingReceivedEvents>();
            await dispatch.Execute();
            await context.WaitAsync(_delay);
        }

        /// <summary>
        /// Ejecuta el procesamiento de los eventos de integracion publicados
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task ProcessPublishedIntegrationEvents(ProcessingContext context)
        {
            _logger.LogInformation("============= Procesando los eventos de integracion publicados ==========");
            context.ThrowIfStopping();
            var dispatch = context.CreateScope()
                .Provider
                .GetRequiredService<DispatchingPublishedEvents>();
            await dispatch.Execute();
            await context.WaitAsync(_delay);
        }

        private async Task ProcessInternalCommands(ProcessingContext context)
        {
            _logger.LogInformation("============= Procesando las reacciones no procesadas ==========");
            context.ThrowIfStopping();
            var dispatch = context.CreateScope()
                .Provider
                .GetRequiredService<InternalCommandProcessing>();
            await dispatch.Execute();
            await context.WaitAsync(_delay);
            
        }

    }
}
