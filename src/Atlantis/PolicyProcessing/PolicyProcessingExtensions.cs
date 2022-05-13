using Atlantis.InternalCommands.Abstractions;
using Atlantis.PolicyProcessing.Abstractions;
using Atlantis.PolicyProcessing.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.PolicyProcessing
{
    /// <summary>
    /// Extension para configurar el procesamiento de las politicas
    /// </summary>
    public static class PolicyProcessingExtensions
    {

        /// <summary>
        /// Registra 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="policyAssembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddPolicyProcessing(this IServiceCollection services, Assembly policyAssembly)
        {
            // Tipo abierto de los descriptores de poliza
            var openPolicyDescriptorType = typeof(AbstractPolicyDescriptor<>);
            // Registramos todos los descriptores de polizas
            policyAssembly.SearchPolicyDescriptor()
                .ForEach(descriptor => services.Add(descriptor));
            // Registramos los procesadores
            //services.AddScoped(typeof(PolicyProcessor<>));
            // Registramos el procesador de comandos CommandPolicyProcessor<T> : ICommandPolicyProcessor<T>
            services.AddScoped(typeof(ICommandPolicyProcessor<>), typeof(CommandPolicyProcessor<>));
            // Registramos el scheduler
            services.AddScoped<IPolicyDispatcher>(sp => sp.GetRequiredService<IInternalCommandScheduler>() as IPolicyDispatcher);
            return services;
        }

        /// <summary>
        /// Carga los descriptores de servicios para despues registrarlos
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static List<ServiceDescriptor> SearchPolicyDescriptor(this Assembly assembly)
        {
            // Obtenemos los tipos
            var openPolicyDescriptorType = typeof(AbstractPolicyDescriptor<>);
            // Query para obtener todos los descriptores de serviicos para las polizas
            var serviceDescriptors = from type in assembly.GetTypes()
                                        where type.IsImplementationClass()
                                        let abstractType = type.GetBaseTypes().Where(IsPolicyDescriptor).FirstOrDefault()
                                        where abstractType is not null
                                        select ServiceDescriptor.Singleton(abstractType, type);
            // Devolvemos la lista
            return serviceDescriptors.ToList();
        }

        /// <summary>
        /// Filtra todas las clases que son implementaciones finales
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsImplementationClass(this Type type) =>
            !type.IsAbstract && !type.IsGenericType && !type.IsInterface;

        /// <summary>
        /// Filtra los tipos que sean genericos y que sean iguales al tipo abstracto
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsPolicyDescriptor(Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AbstractPolicyDescriptor<>);

        /// <summary>
        /// Obtiene las clases base del tipo especificado
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            Type t = type;
            while (true)
            {
                t = t.BaseType;
                if (t == null) break;
                yield return t;
            }
        }

    }
}
