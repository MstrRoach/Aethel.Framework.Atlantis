using Atlantis.Abstractions;
using Atlantis.DomainEvents;
using Atlantis.IntegrationEvents.Abstractions;
using Atlantis.IntegrationEvents.Processor;
using Atlantis.Processing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Internal
{
    /// <summary>
    /// Servidor de tareas en segundo plano
    /// </summary>
    public class AtlantisServer : IProcessingServer
    {
        /// <summary>
        /// Origen de tokens de cancelacion
        /// </summary>
        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// Logger para el servidor
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Fabrica para los procesos hijos
        /// </summary>
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Proveedor de servicios
        /// </summary>
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Tarea para almacenar las demmas tareas
        /// </summary>
        private Task? _compositeTask;

        /// <summary>
        /// Contexto de procesamiento
        /// </summary>
        private ProcessingContext _context = default!;

        /// <summary>
        /// Indica si el servidor ha sido desechado
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Constructor del servidor de atlantis
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="loggerFactory"></param>
        /// <param name="provider"></param>
        public AtlantisServer(ILogger<AtlantisServer> logger,
            ILoggerFactory loggerFactory, IServiceProvider provider)
        {
            _logger = logger;
            _loggerFactory = loggerFactory;
            _provider = provider;
            _cancellationTokenSource = new CancellationTokenSource();
        }
        
        /// <summary>
        /// Ejecuta las tareas del procesador
        /// </summary>
        /// <param name="stoppingToken"></param>
        public void Start(CancellationToken stoppingToken)
        {
            // Registramos que hacer cuando se detenga el proceso
            stoppingToken.Register(() => _cancellationTokenSource.Cancel());
            // Loggeammos
            _logger.LogInformation("Starting the processing server.");
            // Iniciamos el contexto de procesamiento
            _context = new ProcessingContext(_provider, _cancellationTokenSource.Token);
            // Obtenemos las tareas de procesammiento
            var processorTasks = GetProcessors()
                .Select(InfinityRetry)
                .Select(p => p.ProcessAsync(_context));
            // Esperamos a que todas finalicen
            _compositeTask = Task.WhenAll(processorTasks);
        }

        /// <summary>
        /// Obtenemos los procesadores que administrará atlantis
        /// </summary>
        /// <returns></returns>
        private IProcessor[] GetProcessors()
        {
            var processors = new List<IProcessor>()
            {
                _provider.GetRequiredService<DispatchProcessing>()
            };

            return processors.ToArray();
        }

        /// <summary>
        /// Envuelve un proceso para reintentarse infinitamente
        /// </summary>
        /// <param name="inner"></param>
        /// <returns></returns>
        private IProcessor InfinityRetry(IProcessor inner)
        {
            return new InfiniteRetryProcessor(inner, _loggerFactory);
        }

        /// <summary>
        /// Verificamos que el proceso este vivo
        /// </summary>
        public void Pulse()
        {
            _logger.LogTrace("Pulsing the processor.");
        }

        /// <summary>
        /// Sirve para desechar las operaciones principales e hijas
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        public void Dispose()
        {
            if (_disposed) return;

            try
            {
                // Indicamos que fue desechado
                _disposed = true;
                // Logeamos informacion
                _logger.LogInformation("Shutting down the processing server...");
                // Cancelamos todas las tareas
                _cancellationTokenSource.Cancel();
                // Esperamos a que salgan 10 segundos
                _compositeTask?.Wait((int)TimeSpan.FromSeconds(10).TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An exception was occurred when disposing.");
            }
            finally
            {
                _logger.LogInformation("### Atlantis shutdown!");
            }
        }
    }
}
