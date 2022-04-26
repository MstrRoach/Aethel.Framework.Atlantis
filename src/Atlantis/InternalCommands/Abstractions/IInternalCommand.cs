using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands.Abstractions
{
    /// <summary>
    /// Esta interfaz permmite distinguir a los comandos que son
    /// ejecutados por que estan en una cola y mmarcarlos para qie
    /// no se procesen mas de una ves
    /// </summary>
    public interface IInternalCommand
    {
    }
}
