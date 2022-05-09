using Altantis.Console.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console.FirstReaction
{
    internal class FirstReactionHandler : ICommandHandler<FirstReactionCommand, FirstReactionResponse>
    {
        public Task<FirstReactionResponse> Execute(FirstReactionCommand command, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
