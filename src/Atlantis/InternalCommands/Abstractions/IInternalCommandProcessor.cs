using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands.Abstractions
{
    /// <summary>
    /// Servicio que encapsula la logica de procesamiento para
    /// cada comando interno
    /// </summary>
    public interface IInternalCommandProcessor
    {
        /// <summary>
        /// Inicia el procesamiento asincrono de la tarea para ejecutar los comando
        /// </summary>
        /// <returns></returns>
        Task Process();
    }
}
