using Aethel.Extensions.Application.Abstractions.Mediator;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Mediator.Middlewares
{
    internal sealed class Validation<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, ICommand<TResponse>
    {
        /// <summary>
        /// Logger del pipeline
        /// </summary>
        private readonly ILogger<Validation<TRequest, TResponse>> _logger;

        /// <summary>
        /// Lista de validadores para el request que se ejecuta en el contexto
        /// </summary>
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public Validation(ILogger<Validation<TRequest, TResponse>> logger, 
            IEnumerable<IValidator<TRequest>> validators)
        {
            _logger = logger;
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // Obtenemmos info de la solicitud
            var name = request.GetType().Name;
            var id = request.Id.ToString();
            // Creamos la variable de solicitud detallada
            var requestTitle = $"{name} [{id}]";
            _logger.LogInformation("[VALIDATION] {name} Start validation", requestTitle);
            // Si no hay validadores, continuammos con la siguiente capa
            if (!_validators.Any())
                return await next();
            // Creamos un contexto de validacion
            var context = new ValidationContext<TRequest>(request);
            /*
             * Ejecutamos cada validdador, 
             * seleccionamos los errores, 
             * donde los resuultados no sean nulos,
             * Agrupamos errores por propiedad
             * Creammos un diccionario de tipo key, values[]
             */
            var errors = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => x != null)
                .ToList();
            // Si hay errores en el diccionario lanzamos una excepcion de validacion
            if (errors.Any())
            {
                _logger.LogError("[VALIDATION] {name} Validation Failed: {@error}", requestTitle, errors);
                throw new ValidationException(errors);
            }
                
            // Si todo sale bien continuamos con el siguiente
            return await next();
        }
    }
}
