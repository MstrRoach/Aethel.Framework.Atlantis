using Altantis.Console.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console.HelloWorld
{
    public class HelloWorldCommand : ICommand<HelloWorldSaid>
    {
        /// <summary>
        /// Id del evento
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Nombre de quien dice hola
        /// </summary>
        public string Name { get; set; }
    }
}
