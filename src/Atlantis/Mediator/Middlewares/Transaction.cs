using Aethel.Extensions.Application.Abstractions.Data;
using Aethel.Extensions.Application.Abstractions.Mediator;
using Atlantis.DomainEvents.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlantis.Mediator.Middlewares
{
    internal sealed class Transaction<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class, ICommand<TResponse>
    {
        /// <summary>
        /// Contiene la unidad de trabajo del modulo, este es provisto
        /// por el implementador, ya sea a traves de la inyeccion de dependencias
        /// o a traves de una fabrica
        /// </summary>
        private readonly IUnitWork _unitWork;

        /// <summary>
        /// logger para el middleware
        /// </summary>
        private readonly ILogger<Transaction<TRequest, TResponse>> _logger;

        /// <summary>
        /// Permite distribuir los eventos de dominio en el bus de eventos
        /// </summary>
        private readonly IDomainEventProcessor _eventDispatcher;

        /// <summary>
        /// Constuctor del 
        /// </summary>
        /// <param name="unitWork"></param>
        public Transaction(IUnitWork unitWork, ILogger<Transaction<TRequest, TResponse>> logger,
            IDomainEventProcessor eventDispatcher)
        {
            _unitWork = unitWork;
            _logger = logger;
            _eventDispatcher = eventDispatcher;
        }

        /// <summary>
        /// Administra la transaccionalidad
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            try
            {
                _logger.LogInformation("[TRANSACTION] Starting the transaction");
                _unitWork.StartTransaction();
                _logger.LogInformation("[TRANSACTION] Executing request");
                var result = await next();
                // Despachamos los eventos
                _logger.LogInformation("[TRANSACTION] Dispatching events . . .");
                await _eventDispatcher.DispatchEventsAsync(request.Id);
                // TODO:MstrRoach Agregar aqui el despacho de eventos antes de confirmar
                // TODO:MstrRoach Agregar aqui el marcado de los commandos internos commo procesados
                _logger.LogInformation("[TRANSACTION] Request completed successfully. Confirming changes");
                await _unitWork.Commit();
                _logger.LogInformation("[TRANSACTION] Transaction ending.");
                return result;
            }
            catch (Exception ex)
            {
                // Si falla revertimos los cambios
                await _unitWork.Rollback();
                // Loggeamos lo que sucede
                _logger.LogError("[ERROR] Error in transaction. Reverting changes due to: {error}", ex.StackTrace);
                // Lanzamos el error
                throw;
            }
        }
    }
}

