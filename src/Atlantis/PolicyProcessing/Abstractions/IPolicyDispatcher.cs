using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Elemento para despachar las politicas
    /// </summary>
    public interface IPolicyDispatcher
    {
        /// <summary>
        /// Despacha los comando utilizando el mecanismo de despacho determinado
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reaction"></param>
        /// <returns></returns>
        Task Dispatch<T>(object reaction);
    }
}
