using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console
{
    internal class CreateUserCommand
    {
    }

    internal class ProcessDescriptor<Command>
    {
        internal EventDescriptor AlwaysIsLaunch<Event>()
        {
            return new EventDescriptor();
        }
    }

    internal class EventDescriptor
    {
        internal EventDescriptor ThenDo<T>()
        {
            throw new NotImplementedException();
        }
    }
}
