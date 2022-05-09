using Altantis.Console.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console.HelloWorld
{
    public class HelloWorldHandler : ICommandHandler<HelloWorldCommand, HelloWorldSaid>
    {
        public Task<HelloWorldSaid> Execute(HelloWorldCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
