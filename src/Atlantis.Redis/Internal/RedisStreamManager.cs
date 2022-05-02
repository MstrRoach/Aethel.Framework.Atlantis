using Aethel.Redis.Abstractions;
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
    internal class RedisStreamManager : IRedisStreamManager
    {
        /// <summary>
        /// Piscina de conexiones
        /// </summary>
        private readonly IRedisConnectionPool _connectionsPool;

        /// <summary>
        /// Logger del manager
        /// </summary>
        private readonly ILogger<RedisStreamManager> _logger;

        /// <summary>
        /// Opciones de configuracion para redis
        /// </summary>
        private readonly RedisOptions _options;

        /// <summary>
        /// Conexion hacia redis
        /// </summary>
        private IConnectionMultiplexer? _redis;

        /// <summary>
        /// Constructor del administrador de streams
        /// </summary>
        /// <param name="connectionsPool"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public RedisStreamManager(IRedisConnectionPool connectionsPool,
            IOptions<RedisOptions> options, ILogger<RedisStreamManager> logger)
        {
            _options = options.Value;
            _connectionsPool = connectionsPool;
            _logger = logger;
        }

        /// <summary>
        /// Crea un streamm con el grupo consumidor
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="consumerGroup"></param>
        /// <returns></returns>
        public async Task CreateStreamWithConsumerGroupAsync(string stream, string consumerGroup)
        {
            // Realiza la conexion a redis
            await ConnectAsync();

            //The object returned from GetDatabase is a cheap pass - thru object, and does not need to be stored
            var database = _redis!.GetDatabase();
            // Revisamos si una cola existe
            var streamExist = await database.KeyExistsAsync(stream);
            // Si no existe creammos una
            if (!streamExist)
            {
                await database.StreamCreateConsumerGroupAsync(stream, consumerGroup, StreamPosition.NewMessages);
            }
            else
            {
                // Verificamos que grupos estan conectados ala cola
                var groupInfo = await database.StreamGroupInfoAsync(stream);
                // Si ya existe un grupo con el nombre que queremos crear salimos
                if (groupInfo.Any(g => g.Name == consumerGroup))
                    return;
                // Creamos el stream con grupo
                await database.StreamCreateConsumerGroupAsync(stream, consumerGroup, StreamPosition.NewMessages);
            }
        }
        
        /// <summary>
        /// Recupera los ultimos mensajs
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="consumerGroup"></param>
        /// <param name="pollDelay"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<IEnumerable<RedisStream>> PollStreamsLatestMessagesAsync(string[] streams, string consumerGroup, TimeSpan pollDelay, CancellationToken token)
        {
            var positions = streams.Select(stream => new StreamPosition(stream, StreamPosition.NewMessages));

            while (true)
            {
                var result = await TryReadConsumerGroup(consumerGroup, positions.ToArray(), token)
                    .ConfigureAwait(false);

                yield return result;

                token.WaitHandle.WaitOne(pollDelay);
            }
        }

        /// <summary>
        /// Recupera los mensajes pendientes
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="consumerGroup"></param>
        /// <param name="pollDelay"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async IAsyncEnumerable<IEnumerable<RedisStream>> PollStreamsPendingMessagesAsync(string[] streams, string consumerGroup, TimeSpan pollDelay, CancellationToken token)
        {
            var positions = streams.Select(stream => new StreamPosition(stream, StreamPosition.Beginning));

            while (true)
            {
                token.ThrowIfCancellationRequested();

                var result = await TryReadConsumerGroup(consumerGroup, positions.ToArray(), token)
                    .ConfigureAwait(false);

                yield return result;

                //Once we consumed our history of pending messages, we can break the loop.
                if (result.All(s => s.Entries.Length < _options.StreamEntriesCount))
                    break;

                token.WaitHandle.WaitOne(pollDelay);
            }
        }

        /// <summary>
        /// Intenta leer desde un grupo de consumo
        /// </summary>
        /// <param name="consumerGroup"></param>
        /// <param name="positions"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<IEnumerable<RedisStream>> TryReadConsumerGroup(string consumerGroup,
            StreamPosition[] positions, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var createdPositions = new List<StreamPosition>();

                await ConnectAsync();

                var database = _redis!.GetDatabase();

                await foreach (var position in database.TryCreateConsumerGroup(positions, consumerGroup, _logger)
                    .WithCancellation(token))
                    createdPositions.Add(position);

                if (!createdPositions.Any()) return Array.Empty<RedisStream>();

                //calculate keys HashSlots to start reading per HashSlot
                var groupedPositions = createdPositions.GroupBy(s => _redis.GetHashSlot(s.Key))
                    .Select(group => database.StreamReadGroupAsync(group.ToArray(), consumerGroup, consumerGroup, (int)_options.StreamEntriesCount));

                var readSet = await Task.WhenAll(groupedPositions)
                    .ConfigureAwait(false);

                return readSet.SelectMany(set => set);
            }
            catch (OperationCanceledException)
            {
                // ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Redis error when trying read consumer group {consumerGroup}");
            }

            return Array.Empty<RedisStream>();
        }

        /// <summary>
        /// Recuperamos una conexion desde el pool
        /// </summary>
        /// <returns></returns>
        private async Task ConnectAsync()
        {
            _redis = await _connectionsPool.ConnectAsync();
        }

        /// <summary>
        /// Confirma un mmensaje leido
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="consumerGroup"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task Ack(string stream, string consumerGroup, string messageId)
        {
            await ConnectAsync();
            await _redis!.GetDatabase()
                .StreamAcknowledgeAsync(stream, consumerGroup, messageId)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Publica un evento en una cola
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task PublishAsync(string stream, NameValueEntry[] message)
        {
            await ConnectAsync();
            //The object returned from GetDatabase is a cheap pass - thru object, and does not need to be stored
            await _redis!.GetDatabase()
                .StreamAddAsync(stream, message);
        }
    }
}
