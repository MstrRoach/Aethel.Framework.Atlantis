using Aethel.Extensions.Application.Abstractions.Integration;
using Aethel.Extensions.Application.Reflection;
using Aethel.Extensions.Application.Serialization;
using Atlantis.IntegrationEvents.Abstractions;
using Atlantis.IntegrationEvents.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Internal
{
    /// <summary>
    /// Publicador de eventos expuesto a quienes lo usan desde afuera. Este publicador se agrega como
    /// singlenton para temas de concurrencia
    /// </summary>
    internal class DefaultPublisher : IPublisher
    {
        /// <summary>
        /// Proveedor de servicios
        /// </summary>
        private IServiceProvider _serviceProvider;

        /// <summary>
        /// Enviador de los mensajes a traves del transporte definido
        /// </summary>
        private readonly IMessageSender _messageSender;

        /// <summary>
        /// Resolver para los tipos de eventos de integracion disponibles
        /// </summary>
        private readonly TypeManager<IntegrationEvent> _resolver;

        /// <summary>
        /// Almacen para los eventos de salida
        /// </summary>
        private readonly IPublishedEventsStorage _storage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultPublisher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _resolver = serviceProvider.GetRequiredService<TypeManager<IntegrationEvent>>();
            _storage = serviceProvider.GetRequiredService<IPublishedEventsStorage>();
            _messageSender = serviceProvider.GetRequiredService<IMessageSender>();
        }

        /// <summary>
        /// Se encarga de publicar un mensaje en el bus de eventos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : IntegrationEvent
        {
            if(@event is null)
            {
                // Loggeamos algo, y salimos
                return;
            }

            // Construimos el mensaje
            var typeName = @event.GetType().Name;
            // Obtenemos el tipo del evento
            var type = _resolver.GetType(typeName);
            // Serializamos el contenido
            var content = JsonConvert.SerializeObject(@event, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            });
            // Creamos la huella
            var footprint = @event.Id.ToString();
            // Creamos el objeto interno
            var record = new MediumMessage(@event.Id, footprint, typeName, content, @event.OccurredOn);
            // Lo guardamos en el storage
            var message = _storage.StoreMessage(record);
            // Despachamos el registro
            await _messageSender.SendAsync(message);
            // Salimos
            return;
        }
    }
}
