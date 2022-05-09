using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aethel.Redis.Abstractions
{
    internal interface IRedisStreamManager
    {
        /// <summary>
        /// Crea una cola y lo asocia al grupo especificado
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="consumerGroup"></param>
        /// <returns></returns>
        Task CreateStreamWithConsumerGroupAsync(string stream, string consumerGroup);

        /// <summary>
        /// Publica una entrada en la cola especificada
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task PublishAsync(string stream, NameValueEntry[] message);

        /// <summary>
        /// Recupera los ultimos mensajes
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="consumerGroup"></param>
        /// <param name="pollDelay"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        IAsyncEnumerable<IEnumerable<RedisStream>> PollStreamsLatestMessagesAsync(string[] streams, string consumerGroup,
            TimeSpan pollDelay, CancellationToken token);

        /// <summary>
        /// Recupera los mensajes pendientes
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="consumerGroup"></param>
        /// <param name="pollDelay"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        IAsyncEnumerable<IEnumerable<RedisStream>> PollStreamsPendingMessagesAsync(string[] streams, string consumerGroup,
            TimeSpan pollDelay, CancellationToken token);

        /// <summary>
        /// No se quye hace pero vammmmos a ver
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="consumerGroup"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        Task Ack(string stream, string consumerGroup, string messageId);
    }
}
