using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console.Abstractions
{
    /// <summary>
    /// Interface para los comandos que debe devolver siempre una respuesta
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    public interface ICommand<out TResponse> : ICommand { }

    /// <summary>
    /// Interface base para los comandos
    /// </summary>
    public interface ICommand { }
}
