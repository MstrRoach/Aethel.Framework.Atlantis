using Aethel.Extensions.Application.Abstractions.Mediator;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Mediator.Middlewares
{
    /// <summary>
    /// Middleware que nos permite realizar la medicion de tiempos en el que un commando se
    /// ejecuta. Es opcional
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    internal sealed class Performance<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, ICommand<TResponse>
    {
        /// <summary>
        /// Proveedor de logeo de serilog
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Timer para medir los tiempos de ejecucion
        /// </summary>
        private readonly Stopwatch _timer;

        public Performance(ILogger<Performance<TRequest, TResponse>> logger)
        {
            _timer = new Stopwatch();
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // Obtenemmos info de la solicitud
            var name = request.GetType().Name;
            var id = request.Id.ToString();
            // Creamos la variable de solicitud detallada
            var requestTitle = $"{name} [{id}]";
            _timer.Start();
            var response = await next();
            _timer.Stop();
            if (_timer.ElapsedMilliseconds > 500)
            {
                _logger.LogWarning("[PERFORMANCE] {name} Execution time={ElapsedMilliseconds} milliseconds", requestTitle, _timer.ElapsedMilliseconds);
            }
            return response;
        }
    }
}
