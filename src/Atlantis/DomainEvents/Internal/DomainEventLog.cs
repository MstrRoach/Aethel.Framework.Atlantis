using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.DomainEvents.Internal
{
    /// <summary>
    /// Registro de evento de dominio ejecutado con
    /// detalles y para reintentos futuros.
    /// </summary>
    public class DomainEventLog
    {
        /// <summary>
        /// Id del evento, unico por cada evento
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Huella que nos indica cual es el origen y nos permite rastrear los
        /// eventos entre sistemmas, esta huella esta constituida por todos los
        /// id de sus padres
        /// </summary>
        public string Footprint { get; private set; }

        /// <summary>
        /// Indica el tipo de evento para posteriormente deserializar el mismo y construirlo
        /// para ejecucion asincrona
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Evento serializado con los detalles del mismo
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Indica la fecha en la que ocurrio el evento
        /// </summary>
        public DateTime OcurredOn { get; private set; }

        /// <summary>
        /// Indica la fecha en la que se almaceno el evento
        /// </summary>
        public DateTime AddedAt { get; private set; }

        /// <summary>
        /// Indica la fecha en la que se procesó el evento
        /// </summary>
        public DateTime? ProcessedAt { get; private set; }

        /// <summary>
        /// Contiene el registro del error en caso de que el evento no se haya
        /// ejecutado correctammente
        /// </summary>
        public string? Error { get; private set; }

        /// <summary>
        /// Contiene el numero de intentos de ejecucion
        /// </summary>
        public int Retries { get; private set; }

        /// <summary>
        /// Constructor del registro de evento
        /// </summary>
        /// <param name="footprint"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="ocurredOn"></param>
        /// <param name="processedAt"></param>
        /// <param name="error"></param>
        /// <param name="retries"></param>
        public DomainEventLog(string footprint, string type, string content, DateTime ocurredOn,
            DateTime? processedAt = null, string? error = null, int retries = 0)
        {
            this.Id = Guid.NewGuid();
            this.Footprint = footprint;
            this.Type = type;
            this.Content = content;
            this.OcurredOn = ocurredOn;
            this.AddedAt = DateTime.UtcNow;
            this.ProcessedAt = processedAt;
            this.Error = error;
            this.Retries = retries;
        }

        /// <summary>
        /// Marca el evento como procesado
        /// </summary>
        public void WasProcessed() => this.ProcessedAt = DateTime.UtcNow;

        /// <summary>
        /// Marca el mensaje como procesado con error
        /// </summary>
        /// <param name="message"></param>
        public void WasFailed(string message)
        {
            this.ProcessedAt = DateTime.UtcNow;
            this.Error = message;
            this.Retries++;
        }
    }
}
