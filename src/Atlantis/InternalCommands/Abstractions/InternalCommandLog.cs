using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.InternalCommands.Abstractions
{
    /// <summary>
    /// Objeto para almacenar los commandos internos del sistema
    /// </summary>
    public class InternalCommandLog
    {
        /// <summary>
        /// Id del comando
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Tipo de comando para construirlo
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Datos del comando, a saber, su representacion json
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Indica la fecha de creacion
        /// </summary>
        public DateTime EnqueueAt { get; set; }

        /// <summary>
        /// Fecha de procesamiento del comando
        /// </summary>
        public DateTime? ProcessedAt { get; set; }

        /// <summary>
        /// Menssaje de error para cuando no puede publicarse un comando
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="content"></param>
        public InternalCommandLog(Guid id, string type, string content)
        {
            this.Id = id;
            this.Type = type;
            this.Content = content;
            this.EnqueueAt = DateTime.UtcNow;
            this.ProcessedAt = null;
            this.Error = null;
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
        }
    }
}
