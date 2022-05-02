using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Internal
{
   /// <summary>
   /// Estado del evento
   /// </summary>
    public enum EventStatus
    {
        Failed = -1,
        Scheduled,
        Succeeded
    }
}
