using Atlantis.IntegrationEvents.Abstractions;
using Atlantis.IntegrationEvents.Persistence;
using Atlantis.IntegrationEvents.Transport;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Internal
{
    internal class DefaultMessageSender : IMessageSender
    {
        // Estos no se inyectan debido a que algunos de estos se crean utilizando un alcance menor
        private readonly IPublishedEventsStorage _storage;
        private readonly ITransport _transport;

        /// <summary>
        /// Enviador de los mensajes a traves del transporte definido
        /// </summary>
        /// <param name="serviceProvider"></param>
        public DefaultMessageSender(IServiceProvider serviceProvider)
        {
            _storage = serviceProvider.GetRequiredService<IPublishedEventsStorage>();
            _transport = serviceProvider.GetRequiredService<ITransport>();
        }

        /// <summary>
        /// Envuelve la solicitud de envio
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<OperateResult> SendAsync(MediumMessage message)
        {
            // Variable para reintentos
            bool retry;
            // Resultado de la operacion
            OperateResult result;
            // WTF Primera vez que uso un do-while
            do
            {
                // Eejcutamos la operacion sin reintentos
                var executedResult = await SendWithoutRetryAsync(message);
                // Obtenemos el resultado
                result = executedResult.Item2;
                // Si es exitoso, devolvemos el resultado
                if (result == OperateResult.Success)
                    return result;
                // Obtenemos la variable de reintento
                retry = executedResult.Item1;
            } while (retry);
            return result;
        }

        /// <summary>
        /// Se encarga de enviar el mensaje una sola vez y devolver el resultado de la operacion
        /// y una bandera que indique si necesita ser reintentado
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<(bool, OperateResult)> SendWithoutRetryAsync(MediumMessage message)
        {
            // Serializamos el mensaje
            var transportMsg = JsonConvert.SerializeObject(message);
            // Enviamos con la capa de transporte
            var result = await _transport.SendAsync(message);
            // Verificamos el resultado
            if (result.Succeeded)
            {
                // Actualizamos la expiracion
                message.SetSuccessExpiration();
                // Guardamos el storage
                await _storage.UpdateStatusAsync(message, EventStatus.Succeeded);
                // Devolvemos el resultado
                return (false, OperateResult.Success);
            }
            // Incrementamos el contador de reintentos
            message.IncraseRetry();
            // Definimos si necesita reintento
            var needRetry = message.Retries >= 3;
            // Especificamos el error y dentro especificammos la expiracion
            message.SetFailedReason(result.Exception!);
            // almmacenamos
            await _storage.UpdateStatusAsync(message,EventStatus.Failed);
            // retorno
            return (needRetry,OperateResult.Failed(result.Exception!));
        }
    }
}
