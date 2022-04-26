using Aethel.Extensions.Application.Abstractions.Hosting;
using Aethel.Extensions.Application.Abstractions.Mediator;
using Aethel.Extensions.Application.Jobs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Atlantis.Application
{
    public class ModuleHost<Module> : IModuleHost<Module>
    {
        /// <summary>
        /// Contiene todos los servicios disponibles en el modulo
        /// </summary>
        protected IServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Constructor para la inyeccion de dependencias
        /// </summary>
        /// <param name="serviceProvider"></param>
        public ModuleHost(IModuleStartup<Module> startup)
        {
            var services = new ServiceCollection();
            startup.ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Ejecuta un comando con una respuesta
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<TResult> ExecuteCommandAsync<TResult>(ICommand<TResult> command)
        {
            using var scope = ServiceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            return await mediator.Send(command);
        }

        /// <summary>
        /// Ejecuta un comando sin respuesta
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task ExecuteCommandAsync(ICommand command)
        {
            using var scope = ServiceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            await mediator.Send(command);
        }

        /// <summary>
        /// Ejecuta una consulta sin efectos traseros
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public async Task<TResult> ExecuteQueryAsync<TResult>(IQuery<TResult> query)
        {
            using var scope = ServiceProvider.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            return await mediator.Send(query);
        }

        /// <summary>
        /// Ejecuta una accion fuera del entorno de cqrs. Utilizando la interface
        /// de accion de procesamiento, eecutammos logica puntual
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public async Task ExecuteAction<T>() where T : IProcessingAction
        {
            using var scope = ServiceProvider.CreateScope();
            var action = scope.ServiceProvider.GetService<T>();
            if (action is null)
            {
                return;
            }
            await action.Execute();
        }
    }
}
