using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Define un componente que contiene todo lo necesario para
    /// crear un nuevo comando a partir de un evento dado
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public interface IPolicyComponent<TEvent>
    {
        /// <summary>
        /// Permite que la construccion y despacho del comando
        /// utilizando un sistema externo que implemente la interface de despacho
        /// </summary>
        Task DispatchAsync(TEvent @event, IPolicyDispatcher dispatcher);
    }
}
