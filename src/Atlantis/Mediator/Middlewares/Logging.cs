using Aethel.Extensions.Application.Abstractions.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Mediator.Middlewares
{
    /// <summary>
    /// Middleware que nos permite registrar cada comando a traves del logger 
    /// configurado para el sistema
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal sealed class Logging<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, ICommand<TResponse>
    {

        private readonly ILogger<Logging<TRequest, TResponse>> _logger;

        public Logging(ILogger<Logging<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // Obtenemmos info de la solicitud
            var name = request.GetType().Name;
            var id = request.Id.ToString();
            // Creamos la variable de solicitud detallada
            var requestTitle = $"{name} [{id}]";
            try
            {
                _logger.LogInformation($"[START] {requestTitle}");
                _logger.LogInformation("[PROPS] {@Command}", request);
                var response = await next();
                _logger.LogInformation($"[END] {requestTitle}");
                return response;
            }
            catch (Exception ex)
            {
                // Si falla logeamos el fallo explicitamente
                this._logger.LogError(ex, "[ERROR] Command {Command} processing failed", requestTitle);
                throw;
            }
        }
    }
}
