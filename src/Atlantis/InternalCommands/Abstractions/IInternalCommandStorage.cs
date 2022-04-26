using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands.Abstractions
{
    public interface IInternalCommandStorage
    {
        /// <summary>
        /// Agrega un commando 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task AddAsync(InternalCommandLog command);

        /// <summary>
        /// Obtiene todos los comandos que no estan procesados aun
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<InternalCommandLog>> GetUnprocessedCommandsAsync();

        /// <summary>
        /// Actualiza un comando interno para indicar su procesamientro
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        Task UpdateAsync(InternalCommandLog command);

        /// <summary>
        /// Busca un commando a traves del id que tiene definido
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<InternalCommandLog> GetById(Guid id);
    }
}
