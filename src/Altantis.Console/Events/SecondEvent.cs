using Altantis.Console.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altantis.Console.Events
{
    internal class SecondEvent : IDomainEvent
    {
        public int SomeProperty { get; set; }
    }
}
