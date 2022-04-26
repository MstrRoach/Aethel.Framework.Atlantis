using Atlantis.Mediator.Middlewares;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Configuration
{
    internal static class MediatorProvider
    {
        /// <summary>
        /// Configuracion de todo lo concerniente al mediador y sus middlewares, el registro de los handlers y todo lo que respecta 
        /// a lo necesario para habilitar cqrs
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMediator(this IServiceCollection services,params Assembly[] assemblies)
        {

            /*
             * Agregamos el mediador y los handlers para cada comando y query
             */
            services.AddMediatR(assemblies);

            /*
             * Agrega a la lista los middlewares para el pipeline de mediatr y los ejecuta en el
             * orden que se registran
             */
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Logging<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Performance<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Validation<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Transaction<,>));

            /*
             * Agrega las herramientas de validacion fluida y registra todos los validadores
             * que existan al momento de correr la app.
             */

            services.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining(typeof(MediatorProvider));
                fv.DisableDataAnnotationsValidation = true;
            });


            return services;
        }
    }
}
