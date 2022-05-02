using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.IntegrationEvents.Persistence
{
    /// <summary>
    /// Envoltura de los eventos de integracion
    /// </summary>
    public class MediumMessage
    {
        /// <summary>
        /// Id del evento de integracion
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo del evento de integracion
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Contenido serializado del evento de integracion
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Fecha en la que ocurre el evento desde el origen
        /// </summary>
        public DateTime OcurredOn { get; set; }

        /// <summary>
        /// Fecha en la que se agregó al bus de eventos
        /// </summary>
        public DateTime AddedAt { get; set; }

        /// <summary>
        /// Fecha de procesado
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Indica la fecha de expiracion del mensaje
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Contiene el detalle del error
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Indica la cantidad de intentos para distribucion del evento
        /// </summary>
        public int Retries { get; set; }

        /// <summary>
        /// Constructor de la envoltura para los eventos
        /// </summary>
        /// <param name="id"></param>
        /// <param name="footprint"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        /// <param name="ocurredOn"></param>
        public MediumMessage(Guid id, string footprint, string type, string content,
            DateTime ocurredOn)
        {
            this.Id = id;
            this.Type = type;
            this.Content = content;
            this.OcurredOn = ocurredOn;
            this.AddedAt = DateTime.Now.ToUniversalTime();
            this.ProcessedAt = null;
            this.Error = null;
            this.Retries = 0;
        }

        [JsonConstructor]
        public MediumMessage()
        {

        }

        /// <summary>
        /// Incrementa el contador de reintentos
        /// </summary>
        public void IncraseRetry()
        {
            this.Retries++;
        }

        /// <summary>
        /// Especifica el error a partir de una excepcion
        /// </summary>
        /// <param name="exception"></param>
        public void SetFailedReason(Exception exception)
        {
            this.Error = $"{exception.GetType().Name}-->{exception.Message}";
            this.ExpiresAt = this.AddedAt.AddDays(15);
        }

        /// <summary>
        /// Pone la expiracion para cuando el mensaje se proceso de forma exitosa
        /// </summary>
        public void SetSuccessExpiration()
        {
            this.ExpiresAt = DateTime.UtcNow.AddSeconds(24*3600);
        }
    }
}
