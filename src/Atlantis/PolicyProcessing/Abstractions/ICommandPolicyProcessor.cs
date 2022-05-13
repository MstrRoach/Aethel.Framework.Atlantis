using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Contiene lo necesario para hacer el procesamiento de las polizas
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICommandPolicyProcessor<T>
    {
        /// <summary>
        /// Ejecuta la poliza en un ciclo
        /// </summary>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        Task ExecutePolicy(Guid sourceId);
    }
}
