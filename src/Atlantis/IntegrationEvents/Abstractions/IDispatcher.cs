using Atlantis.IntegrationEvents.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Abstractions
{
    /// <summary>
    /// Se encarga de despachar y encolar los eventos en memoria para reintentarlos
    /// </summary>
    public interface IDispatcher
    {
        /// <summary>
        /// Encola un mensaje para que sea publicado
        /// </summary>
        /// <param name="message"></param>
        void EnqueueToPublish(MediumMessage message);

        /// <summary>
        /// Encola un mensaje para que sea ejecutado
        /// </summary>
        /// <param name="message"></param>
        void EnqueueToExecute(MediumMessage message);
    }
}
