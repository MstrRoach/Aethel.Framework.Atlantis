using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console.Abstractions
{
    /// <summary>
    /// Interface para todos los handlers de comandos
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public interface ICommandHandler<in TCommand,TResponse> : ICommandHandler
        where TCommand : ICommand<TResponse>
    {
        /// <summary>
        /// Ejecuta la logica de negocio del comando
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResponse> Execute(TCommand command, CancellationToken cancellationToken);
    }

    /// <summary>
    /// Interface para las operaciones de construccion
    /// </summary>
    public interface ICommandHandler { }
}
