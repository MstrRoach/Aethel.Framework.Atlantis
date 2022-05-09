using Aethel.Structure.ExplicitFlow.Internal;
using Altantis.Console.Abstractions;
using Altantis.Console.Events;
using Altantis.Console.FirstReaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console.HelloWorld
{
    /// <summary>
    /// Politica desarrollada para el comando
    /// </summary>
    public class HelloWorldPolicy : AbstractPolicyDescriptor<HelloWorldCommand>
    {

        public HelloWorldPolicy()
        {
            // Especificamos la politica para el evento de usuario creado
            policyBuilder.When<UserCreatedEvent>()
                .ThenDo(evt => new SendWelcomeEmailCommand(evt.SomeProperty))
                .ThenDo(evt => new PublishUserCreated(evt.SomeProperty));
            //// Especificamos la politica para el primer evento
            //policyBuilder.When<FirstEvent>()
            //    .ThenDo(evt => new FirstReactionCommand(evt.SomeProperty));
        }
    }

}
