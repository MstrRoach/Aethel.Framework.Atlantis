using Atlantis.Configuration;
using Atlantis.IntegrationEvents.Abstractions;
using Atlantis.IntegrationEvents.Persistence;
using Atlantis.IntegrationEvents.Transport;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Internal
{
    internal class DefaultConsumerRegister : IConsumerRegister
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;
        private readonly TimeSpan _pollingDelay = TimeSpan.FromSeconds(1);

        private IReceivedEventsStorage _storage;
        private IConsumerClientFactory _consumerClientFactory = default!;
        private IntegrationOptions _options;

        private CancellationTokenSource _cts = new();
        private Task? _compositeTask;
        private bool _isHealthy = true;
        private bool _disposed;

        public DefaultConsumerRegister(IServiceProvider serviceProvider, 
            ILogger<DefaultConsumerRegister> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Inicia el procesamiento del consummidor
        /// </summary>
        /// <param name="stoppingToken"></param>
        public void Start(CancellationToken stoppingToken)
        {
            _options = _serviceProvider.GetRequiredService<IOptions<IntegrationOptions>>().Value;
            _consumerClientFactory = _serviceProvider.GetRequiredService<IConsumerClientFactory>();
            _storage = _serviceProvider.GetRequiredService<IReceivedEventsStorage>();
            stoppingToken.Register(Dispose);
            Execute();
        }

        /// <summary>
        /// Ejecuta la logica para el procesador
        /// </summary>
        public void Execute()
        {
            // Lista de grupos con los topicos
            var consumerGroups = new Dictionary<string, List<string>>
            {
                { _options.DefaultGroup, _options.Subscriptions }
            };

            // Recorremos los grupos donde se conectan
            foreach (var group in consumerGroups)
            {
                // Lista de topics
                ICollection<string> topics;
                try
                {
                    // Creamos los topics a los que se conecta
                    using var client = _consumerClientFactory.Create(group.Key);
                    topics = client.FetchTopics(group.Value);
                }catch (Exception ex)
                {
                    _isHealthy = false;
                    _logger.LogError(ex, ex.Message);
                    return;
                }
                var topicIds = topics.Select(t => t);
                // Comenzamos a configurar los escuchadores
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        using var client = _consumerClientFactory.Create(group.Key);
                        RegisterMessageProcessor(client);
                        client.Subscribe(topicIds);
                        client.Listening(_pollingDelay, _cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        //ignore
                    }
                    catch (Exception ex)
                    {
                        _isHealthy = false;
                        _logger.LogError(ex, ex.Message);
                    }
                }, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            }
            Console.WriteLine("### Operacion de consumo en proceso");
        }

        /// <summary>
        /// Define el comportamiento cuando se recibe un mensaje
        /// </summary>
        /// <param name="client"></param>
        private void RegisterMessageProcessor(IConsumerClient client)
        {
            client.OnMessageReceived += (sender, message) =>
            {
                try
                {
                    _logger.LogInformation("## Message received with ID {0}",message.Id);
                    // Guardamos el mensaje 
                    _storage.StoreMessage(message);
                    // Confirmamos el mensaje
                    client.Commit(sender);
                }catch(Exception ex)
                {
                    _logger.LogError(ex, "An exception occurred when process received message. Message:'{0}'.", message);
                    // Rechazamos el mensaje
                    client.Reject(ex);
                }
            };
        }

        /// <summary>
        /// Reinicia los servicios y las operaciones
        /// </summary>
        /// <param name="force"></param>
        public void ReStart(bool force = false)
        {
            if (!IsHealthy() || force)
            {
                // Cancela toda las operaciones
                Pulse();
                _cts = new CancellationTokenSource();
                _isHealthy = true;
                // Inicia la ejecucion de nuevo
                Execute();
            }
        }

        /// <summary>
        /// Cancela la operacion y la desecha
        /// </summary>
        public void Pulse()
        {
            _cts.Cancel();
            _cts.Dispose();
        }

        /// <summary>
        /// Devuelve si el procesador actual es correcto
        /// </summary>
        /// <returns></returns>
        public bool IsHealthy()
        {
            return _isHealthy;
        }

        /// <summary>
        /// Desecha los recursos administrados
        /// </summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try
            {
                Pulse();

                _compositeTask?.Wait(TimeSpan.FromSeconds(2));
            }
            catch (AggregateException ex)
            {
                var innerEx = ex.InnerExceptions[0];
                //if (!(innerEx is OperationCanceledException))
                //{
                //    _logger.ExpectedOperationCanceledException(innerEx);
                //}
            }
        }
    }
}
