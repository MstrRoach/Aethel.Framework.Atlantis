using Aethel.Extensions.Application.Abstractions.Data;
using Atlantis.DomainEvents.Abstractions;
using Atlantis.InternalCommands.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Configuration
{
    /// <summary>
    /// Define las opciones configurables para utilizar atlantis o para
    /// habilitar desde la configuracion del modulo
    /// </summary>
    public class AtlantisBuilder
    {
        /// <summary>
        /// Servicios donde se registran las configuraciones
        /// </summary>
        private readonly IServiceCollection _services;

        /// <summary>
        /// Bandera para saber si la uniddad de trabajo fue configurada
        /// </summary>
        private bool unitWorkIsConfigured = false;

        /// <summary>
        /// Bandera para saber si se especifico el servicio de almacenamiento para los
        /// registros de eventos
        /// </summary>
        private bool eventLogStorageIsConfigured = false;

        /// <summary>
        /// Bandera para saber si se especifico el servicio de almacenamiento para los
        /// comandos internos
        /// </summary>
        private bool internalCommandStorageIsConfigured = false;

        /// <summary>
        /// Indica el ensamblado donde se ubican los handlers para
        /// implementar el mediador
        /// </summary>
        public Assembly? HandlersAssembly { get; private set; }

        /// <summary>
        /// Indica el ensamblado donde se ubican los eventos de dominio
        /// para buscar y tener en cuenta al momento de procesarlos
        /// </summary>
        public Assembly? DomainEventsAssembly { get; private set; }

        /// <summary>
        /// Indica el ensamblado donde se ubican los comandos internos
        /// para buscarlos y tenerlos en cuenta en el procesamiento
        /// </summary>
        public Assembly? InternalCommandsAssembly { get; private set; }

        /// <summary>
        /// Configuraciones obligatorias para que atlantis funcione
        /// </summary>
        /// <param name="services"></param>
        public AtlantisBuilder(IServiceCollection services)
        {
            _services = services;
        }

        /// <summary>
        /// Especifica la clase que se usa commo unidad de trabajo en las
        /// operaciones del modulo con atlantis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UseUnitWork<T>() where T : IUnitWork
        {
            _services.Add(ServiceDescriptor.Scoped(typeof(IUnitWork), typeof(T)));
            unitWorkIsConfigured = true;
        }

        /// <summary>
        /// Especifica y registra la entidad para almacenar los eventos
        /// de dominio
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UseDomainEventStorage<T>() where T : IDomainEventLogStorage
        {
            _services.Add(ServiceDescriptor.Scoped(typeof(IDomainEventLogStorage),typeof(T)));
            eventLogStorageIsConfigured = true;
        }

        /// <summary>
        /// Registra la entidad utilizada para almacenar los comandos internos
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void UseInternalCommandStorage<T>() where T : IInternalCommandStorage
        {
            _services.Add(ServiceDescriptor.Scoped(typeof(IInternalCommandStorage), typeof(T)));
            internalCommandStorageIsConfigured = true;
        }

        /// <summary>
        /// Especifica el ensamblado donde buscar los handlers para los comandos y queries
        /// </summary>
        /// <param name="assembly"></param>
        public void SetHandlerLocation(Assembly assembly)
        {
            this.HandlersAssembly = assembly;
        }

        /// <summary>
        /// Indica el ensamblado donde buscar los eventos de domminio
        /// </summary>
        /// <param name="assembly"></param>
        public void SetDomainEventLocation(Assembly assembly)
        {
            this.DomainEventsAssembly = assembly;
        }

        /// <summary>
        /// Indica el ensammblado donde buscar los commandos internos
        /// </summary>
        /// <param name="assembly"></param>
        public void SetInternalCommandLocation(Assembly assembly)
        {
            this.InternalCommandsAssembly = assembly;
        }

        /// <summary>
        /// Verifica que todas las configuraciones han sido especifidas, en 
        /// caso contraro debe de lanzar una excepcion
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void IsConfigured()
        {
            if (!unitWorkIsConfigured)
                throw new InvalidOperationException("Unit work must be defined");
            if(!eventLogStorageIsConfigured)
                throw new InvalidOperationException("Event storage must be defined");
            if (!internalCommandStorageIsConfigured)
                throw new InvalidOperationException("Internal command storage must be defined");
            if (HandlersAssembly is null)
                throw new InvalidOperationException("Handlers assembly must be specified");
            if (DomainEventsAssembly is null)
                throw new InvalidOperationException("Domain Events assembly must be specified");
            if (InternalCommandsAssembly is null)
                throw new InvalidOperationException("Internal commands assembly must be specified");
        }

    }
}
