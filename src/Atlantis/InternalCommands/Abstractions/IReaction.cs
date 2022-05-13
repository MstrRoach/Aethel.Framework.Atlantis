using Aethel.Extensions.Application.Abstractions.Mediator;
using Aethel.Extensions.Application.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing.Abstractions
{
    /// <summary>
    /// Interface para identificar a los commandos que
    /// son ejecutados a partir de un evento especificado
    /// </summary>
    public interface IReaction : ICommand<Answer<Unit>>
    {

    }
}
