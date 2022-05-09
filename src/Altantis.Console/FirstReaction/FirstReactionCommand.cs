using Altantis.Console.Abstractions;
using Altantis.Console.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Altantis.Console.FirstReaction
{
    internal class FirstReactionCommand : ICommand<FirstReactionResponse>, IReaction<FirstEvent>
    {

        /// <summary>
        /// Propiedad especificada
        /// </summary>
        public string SomeProperty { get; set; }

        /// <summary>
        /// Constructor del comando 
        /// </summary>
        /// <param name="someProperty"></param>
        [JsonConstructor]
        public FirstReactionCommand(string someProperty)
        {
            this.SomeProperty = someProperty;
        }
    }
}
