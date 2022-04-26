using Aethel.Extensions.Application.Jobs;
using Atlantis.InternalCommands.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands
{
    public class InternalCommandProcessing : IProcessingAction
    {
        private readonly IInternalCommandProcessor _commandProcessor;
        public InternalCommandProcessing(IInternalCommandProcessor commandProcessor)
        {
            _commandProcessor = commandProcessor;
        }

        public async Task Execute()
        {
            await _commandProcessor.Process();
        }
    }
}
