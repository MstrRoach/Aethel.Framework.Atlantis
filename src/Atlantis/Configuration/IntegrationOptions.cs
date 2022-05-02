using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Configuration
{
    /// <summary>
    /// Contiene configuraciones para el tema de integracion
    /// </summary>
    public class IntegrationOptions
    {
        /// <summary>
        /// Indica la cola o stream donde publicara los eventos
        /// </summary>
        public string DefaultStream { get; set; } = "DefaultModuleStream";

        /// <summary>
        /// Lista de streams donde se suscribira el modulo para escuchar
        /// </summary>
        public List<string> Subscriptions { get; set; } = new List<string>();

        /// <summary>
        /// Indica el nombre del grupo donde recibira los eventos, la bandeja de entrada
        /// </summary>
        public string DefaultGroup { get; set; } = "DefaultModuleGroup";
    }
}
