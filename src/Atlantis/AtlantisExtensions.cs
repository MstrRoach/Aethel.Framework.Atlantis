using Aethel.Extensions.Application.Abstractions.Data;
using Aethel.Extensions.Application.Abstractions.Hosting;
using Aethel.Extensions.Application.Jobs;
using Aethel.Extensions.Application.Reflection;
using Aethel.Extensions.Domain;
using Atlantis.Application;
using Atlantis.Configuration;
using Atlantis.DomainEvents;
using Atlantis.DomainEvents.Abstractions;
using Atlantis.DomainEvents.Internal;
using Atlantis.InternalCommands;
using Atlantis.InternalCommands.Abstractions;
using Atlantis.InternalCommands.Internal;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System.Reflection;

namespace Atlantis
{
    public static class AtlantisExtensions
    {
        
        /// <summary>
        /// Agrega el framework con configuraciones personalidas. Este debe de agregarse en el modulo donde
        /// se implementará todo lo necessario para funcionar en alcance modular. 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection AddAtlantis(this IServiceCollection services, Action<AtlantisBuilder> configuration)
        {
            // Si no hay configuracion devolvemmos error
            if(configuration is null)
                throw new ArgumentNullException(nameof(configuration));
            // Creamos el contenedor de configuracion
            var options = new AtlantisBuilder(services);
            // Llenamos las configuraciones
            configuration(options);
            // Verificamos las configuraciones
            options.IsConfigured();
            // Obtenemos el ensammblado actual
            var internalAssembly = typeof(AtlantisExtensions).GetTypeInfo().Assembly;
            // Configuramos el mediador
            services.AddMediator(options.HandlersAssembly,internalAssembly);
            // Configuramos el despachador de eventos de dominio
            services.AddScoped<IDomainEventCollector, DomainEventCollector>();
            services.AddScoped<IDomainEventProcessor, DomainEventProcessor>();
            services.AddSingleton(new TypeManager<IDomainEvent>(options.DomainEventsAssembly));
            services.AddScoped<DomainEventProcessing>();
            // La configuracion externa registra el IDomainEventLogStorage
            // Configuramos el procesador de eventos internos
            services.AddScoped<IInternalCommandScheduler, InternalCommandScheduler>();
            services.AddScoped<IInternalCommandProcessor, InternalCommandProcessor>();
            services.AddSingleton(new TypeManager<IInternalCommand>(options.InternalCommandsAssembly));
            services.AddScoped<InternalCommandProcessing>();
            // La configuracion externa registra el IInternalCommandStorage
            return services;
        }

        /// <summary>
        /// Registra el startup y el host del modulo para estar disponible en la aplicacion y el contenedor
        /// principal
        /// </summary>
        /// <typeparam name="Module"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAtlantisModule<Module>(this IServiceCollection services)
        {
            var types = typeof(Module).Assembly.GetTypes();
            var startupImpl = types
                            .Where(type => type.GetInterfaces()
                            .Any(i => 
                                i.IsGenericType &&
                                i.GetGenericTypeDefinition() == typeof(IModuleStartup<>) &&
                                i.GenericTypeArguments.Single() == typeof(Module))).FirstOrDefault();

            var startupDescriptor = ServiceDescriptor.Singleton(typeof(IModuleStartup<Module>), startupImpl);

            services.Add(startupDescriptor);
            services.AddSingleton<IModuleHost<Module>, ModuleHost<Module>>();
            return services;
        }

        /// <summary>
        /// Registra los jobs registrados y listados dentro de la lista de definicion de jobs, asi como los jobs
        /// encontrados dentro del ensammblado y se registran en el servicio origen
        /// </summary>
        /// <typeparam name="Module"></typeparam>
        /// <param name="quartz"></param>
        /// <param name="services"></param>
        /// <param name="jobDefinitions"></param>
        /// <returns></returns>
        public static IServiceCollectionQuartzConfigurator AddAtlantisJobs<Module>(this IServiceCollectionQuartzConfigurator quartz, IServiceCollection services, List<JobDefinition> jobDefinitions)
        {
            // Buscamos los jobs existentes
            var declaredJobs = typeof(Module)
                .GetTypeInfo()
                .Assembly.GetTypes()
                .Where(x =>
                    x.GetInterface(nameof(IJob)) is not null)
                .ToList();

            // Recorremos las definiciones de jobs
            foreach (var jobDefinition in jobDefinitions)
            {
                // Si el job esta inactivo, no lo configuramos
                if (!jobDefinition.IsActive)
                    continue;
                // Si el job no pertenece a este modulo
                if (!jobDefinition.Group.Equals(typeof(Module).Name))
                    continue;
                // Buscamos el tipo en la lista de jobs existentes
                var jobType = declaredJobs.Where(x => x.FullName == jobDefinition.Name).FirstOrDefault();
                // Si no se encuentra
                if (jobType is null)
                {
                    // Logeamos que no existe, salimos
                    continue;
                }
                // Registramos el job en el contenedor externo
                services.AddTransient(jobType);
                // Creamos la clave del job
                var jobKey = new JobKey(jobDefinition.Key, jobDefinition.Group);
                // Agregammos el job
                quartz.AddJob(jobType, jobKey, opts => opts.WithDescription(jobDefinition.Description));
                // Agregamos el trigger
                quartz.AddTrigger(opts =>
                {
                    opts.ForJob(jobKey);
                    opts.WithIdentity($"{jobDefinition.Key}-trigger");
                    opts.WithCronSchedule(jobDefinition.Cron);
                });
                // Continuamos
                continue;
            }

            return quartz;
        }

    }
}