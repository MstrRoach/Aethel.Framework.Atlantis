using Aethel.Extensions.Application.Reflection;
using Atlantis.InternalCommands.Abstractions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands.Internal
{
    internal class InternalCommandProcessor : IInternalCommandProcessor
    {
        private readonly IServiceProvider _provider;
        private readonly IInternalCommandStorage _storage;
        private readonly TypeManager<IInternalCommand> _resolver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediator"></param>
        /// <param name="storage"></param>
        /// <param name="resolver"></param>
        public InternalCommandProcessor(IServiceProvider provider, IInternalCommandStorage storage, 
            TypeManager<IInternalCommand> resolver)
        {
            _provider = provider;
            _storage = storage;
            _resolver = resolver;
        }

        /// <summary>
        /// Carga los comandos almmacenados sin procesar y los procesa a traves de la instancia de
        /// mediator para habilitar el comportamiento reactivo
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task Process()
        {
            var commandLogs = await _storage.GetUnprocessedCommandsAsync();
            if (!commandLogs.Any())
                return;

            foreach (var commandLog in commandLogs)
            {
                var type = _resolver.GetType(commandLog.Type);
                if (type is null)
                {
                    commandLog.WasFailed("Tipo del evento no encontrado");
                    await _storage.UpdateAsync(commandLog);
                    continue;
                }
                var command = JsonConvert.DeserializeObject(commandLog.Content, type);
                if (command is null)
                {
                    commandLog.WasFailed("No se pudo deserializar el contenido");
                    await _storage.UpdateAsync(commandLog);
                    continue;
                }

                try
                {
                    using var scope = _provider.CreateScope();
                    var mediator = scope.ServiceProvider.GetService<IMediator>();
                    await mediator.Send(command);
                    commandLog.WasProcessed();
                    await _storage.UpdateAsync(commandLog);
                }catch (Exception ex)
                {
                    commandLog.WasFailed(ex.Message);
                    await _storage.UpdateAsync(commandLog);
                    continue;
                }
            }
        }
    }
}
