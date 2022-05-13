using Aethel.Redis.Abstractions;
using Atlantis.IntegrationEvents.Persistence;
using Atlantis.IntegrationEvents.Transport;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Internal
{
    internal class RedisConsumerClient : IConsumerClient
    {
        /// <summary>
        /// Grupo al que el cliente leera los mensajes
        /// </summary>
        private readonly string _groupId;

        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<RedisConsumerClient> _logger;

        /// <summary>
        /// Opciones de configuracion
        /// </summary>
        private readonly IOptions<RedisOptions> _options;

        /// <summary>
        /// Administra el strem 
        /// </summary>
        private readonly IRedisStreamManager _redis;

        /// <summary>
        /// Lista de temas que escuchará
        /// </summary>
        private string[] _topics = default!;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="groupId"></param>
        /// <param name="redis"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public RedisConsumerClient(string groupId,
            IRedisStreamManager redis,
            IOptions<RedisOptions> options,
            ILogger<RedisConsumerClient> logger)
        {
            _groupId = groupId;
            _redis = redis;
            _options = options;
            _logger = logger;
        }


        public event EventHandler<MediumMessage> OnMessageReceived;

        /// <summary>
        /// Subscribe el grupo a la lista de temmas
        /// </summary>
        /// <param name="topics"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Subscribe(IEnumerable<string> topics)
        {
            if(topics is null) 
                throw new ArgumentNullException(nameof(topics));
            // Suscribimos el grupo a las colas
            foreach (var topic in topics)
            {
                _redis.CreateStreamWithConsumerGroupAsync(topic, _groupId)
                    .GetAwaiter()
                    .GetResult();
            }

            // Conservamos las colas o las referencias a estas
            _topics = topics.ToArray();
        }

        public void Listening(TimeSpan timeout, CancellationToken cancellationToken)
        {
            _ = ListeningForMessagesAsync(timeout, cancellationToken);
        }

        /// <summary>
        /// Relaiza la ejecucion para escuchar los menssajes de la cola
        /// </summary>
        /// <param name="timeout"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task ListeningForMessagesAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            // Primero recuperammos los mensajes pendientes en caso de crasheo y recuperacion
            var pendingMessages = _redis.PollStreamsLatestMessagesAsync(_topics, _groupId, timeout, cancellationToken);
            // Consumimmmos los mensajes
            await ConsumeMessages(pendingMessages, StreamPosition.Beginning);

            // Comenzamos a obtener nuevos mensajes
            var newMsgs = _redis.PollStreamsLatestMessagesAsync(_topics,_groupId,timeout,cancellationToken);
            // Consumimos los mensajes
            _ = ConsumeMessages(newMsgs, StreamPosition.NewMessages);
        }

        /// <summary>
        /// Realiza el consummo de los mensajes 
        /// </summary>
        /// <param name="streamsSet"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        private async Task ConsumeMessages(IAsyncEnumerable<IEnumerable<RedisStream>> streamsSet, RedisValue position)
        {
            // Recorremos el set de streams
            await foreach (var set in streamsSet)
            {
                // Recorremos cada strem
                foreach (var stream in set)
                {
                    // Recorremos cada entrada
                    foreach(var entry in stream.Entries)
                    {
                        // Si es nula salimos
                        if (entry.IsNull) return;
                        try
                        {
                            // Deserializammos
                            var message = RedisMessage.Create(entry);
                            // Invocamos el metodo para procesarlo
                            OnMessageReceived?.Invoke((stream.Key.ToString(), _groupId, entry.Id.ToString()), message);
                        }catch (Exception ex)
                        {
                            _logger.LogError(ex.Message, ex);
                        }
                        finally
                        {
                            var positionName = position == StreamPosition.Beginning
                                ? nameof(StreamPosition.Beginning)
                                : nameof(StreamPosition.NewMessages);
                            _logger.LogDebug($"Redis stream entry [{entry.Id}] [position : {positionName}] was delivered.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Confirma la recepcion de un mensaje para mmarcarlo como procesado
        /// </summary>
        /// <param name="sender"></param>
        public void Commit(object sender)
        {
            var (stream, group, id) = ((string stream, string group, string id))sender;
            _redis.Ack(stream,group,id);
        }

        /// <summary>
        /// Libera los recursos
        /// </summary>
        public void Dispose()
        {
            // Ignore
        }

        /// <summary>
        /// Indica al cliente que el mensaje no fue procesado correctamente
        /// </summary>
        /// <param name="sender"></param>
        public void Reject(object? sender)
        {
            // Ignore
        }
    }
}
