using Aethel.Extensions.Application.Abstractions.Mediator;
using Aethel.Extensions.Application.Serialization;
using Atlantis.InternalCommands.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands.Internal
{
    /// <summary>
    /// Servicio para encolar todos los comandos necesarios y almacenarlos en la base de datos
    /// </summary>
    internal class InternalCommandScheduler : IInternalCommandScheduler
    {
        private readonly IInternalCommandStorage _storage;
        public InternalCommandScheduler(IInternalCommandStorage storage)
        {
            _storage = storage;
        }

        /// <summary>
        /// Agrega un comando a la cola de ejecucion
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task EnqueueAsync(ICommand command)
        {
            var type = command.GetType().FullName;
            var content = JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            });

            var commandLog = new InternalCommandLog(command.Id, type, content);
            await _storage.AddAsync(commandLog);
        }

        /// <summary>
        /// Agrega un comando a la cola de ejecucion
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task EnqueueAsync<T>(ICommand<T> command)
        {
            var type = command.GetType().FullName;
            var content = JsonConvert.SerializeObject(command, new JsonSerializerSettings
            {
                ContractResolver = new AllPropertiesContractResolver()
            });

            var commandLog = new InternalCommandLog(command.Id, type, content);
            await _storage.AddAsync(commandLog);
        }

        /// <summary>
        /// Marca un commmando interno como completo siemmmpre y cuando
        /// sea uno
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task MarkAsDone<TResponse>(ICommand<TResponse> command)
        {
            if (command is not IInternalCommand)
                return;

            var commandLog = await _storage.GetById(command.Id);
            if (commandLog is null)
                return;

            commandLog.WasProcessed();
            await _storage.UpdateAsync(commandLog);
        }
    }
}
