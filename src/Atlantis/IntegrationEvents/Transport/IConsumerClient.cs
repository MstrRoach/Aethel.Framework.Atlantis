using Atlantis.IntegrationEvents.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Transport
{
    /// <summary>
    /// Cliente de conexion con los buses de eventos
    /// </summary>
    public interface IConsumerClient : IDisposable
    {
        /// <summary>
        /// Crea si es necesario los topics pasados y los retorna
        /// </summary>
        /// <param name="topicNames">Names of the requested topics</param>
        /// <returns>Topic identifiers</returns>
        ICollection<string> FetchTopics(IEnumerable<string> topicNames)
        {
            return topicNames.ToList();
        }

        /// <summary>
        /// Subscribe to a set of topics to the message queue
        /// </summary>
        /// <param name="topics"></param>
        void Subscribe(IEnumerable<string> topics);

        /// <summary>
        /// Comienza el proceso de escucha
        /// </summary>
        void Listening(TimeSpan timeout, CancellationToken cancellationToken);

        /// <summary>
        /// Manual submit message offset when the message consumption is complete
        /// </summary>
        void Commit(object sender);

        /// <summary>
        /// Reject message and resumption
        /// </summary>
        void Reject(object? sender);

        /// <summary>
        /// Evento para cuando se recibe un mensaje
        /// </summary>
        event EventHandler<MediumMessage> OnMessageReceived;

        //event EventHandler<LogMessageEventArgs> OnLog;
    }
}
