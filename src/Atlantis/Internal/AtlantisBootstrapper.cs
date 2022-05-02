using Aethel.Extensions.Application.Abstractions.Hosting;
using Atlantis.Abstractions;
using Atlantis.IntegrationEvents.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Internal
{
    internal class AtlantisBootstrapper<Module> : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private IEnumerable<IProcessingServer> _processors = default!;
        private readonly CancellationTokenSource _cts = new();
        private bool _disposed;
        public AtlantisBootstrapper(IModuleHost<Module> host, ILogger<AtlantisBootstrapper<Module>> logger)
        {
            _serviceProvider = host.GetServiceProvider();
            _logger = logger;
        }

        public async Task BootstrapAsync()
        {
            _logger.LogDebug("### Atlantis background task is starting.");

            try
            {
                // Cargamos los procesadores
                _processors = _serviceProvider.GetServices<IProcessingServer>();
            }
            catch (Exception ex)
            {
                _logger.LogDebug("### Error to load processors.");
            }

            // Registramos la accion para cuando se detenga el procesamiento
            _cts.Token.Register(() =>
            {
                _logger.LogDebug("### Atlantis background task is stopping.");

                foreach (var item in _processors)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogWarning(ex, $"Expected an OperationCanceledException, but found '{ex.Message}'.");
                    }
                }
            });

            // Iniciamos el proceso central
            await BootstrapCoreAsync();

            _logger.LogInformation("### Atlantis started!");
        }

        /// <summary>
        /// Procesamiento central
        /// </summary>
        /// <returns></returns>
        protected virtual Task BootstrapCoreAsync()
        {
            // Iniciamos el proceso en cada procesador
            foreach (var item in _processors)
            {
                _cts.Token.ThrowIfCancellationRequested();

                try
                {
                    item.Start(_cts.Token);
                }
                catch (OperationCanceledException)
                {
                    // ignore
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Starting the processors throw an exception.");
                }
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Desechamos todo el proceso
        /// </summary>
        public override void Dispose()
        {
            if (_disposed)
            {
                return;
            }
            _cts.Cancel();
            _cts.Dispose();
            _disposed = true;
        }


        /// <summary>
        /// Inicio del proceso
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await BootstrapAsync();
        }

        /// <summary>
        /// Fin del proceso
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _cts.Cancel();

            await base.StopAsync(cancellationToken);
        }
    }
}
