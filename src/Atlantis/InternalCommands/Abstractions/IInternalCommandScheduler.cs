using Aethel.Extensions.Application.Abstractions.Mediator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands.Abstractions
{
    /// <summary>
    /// Este es quien programa y encola los comandos de
    /// sistema en el almacen para procesarlos despues
    /// </summary>
    public interface IInternalCommandScheduler
    {
        /// <summary>
        /// Agrega un comando a la cola de de ejecucion
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task EnqueueAsync(ICommand command);

        /// <summary>
        /// Agrega un comando con respuesta a la cola de ejecucion
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        Task EnqueueAsync<T>(ICommand<T> command);

        /// <summary>
        /// Permmite marcar a los comandos commmo procesados
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        Task MarkAsDone<TResponse>(ICommand<TResponse> command);
    }
}
