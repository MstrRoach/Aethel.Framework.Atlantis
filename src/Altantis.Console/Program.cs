// See https://aka.ms/new-console-template for more information
using Aethel.Structure.ExplicitFlow.Internal;
using Altantis.Console;
using Altantis.Console.HelloWorld;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine("Hello, World!");

var services = new ServiceCollection();

services.AddSingleton<AbstractPolicyDescriptor<HelloWorldCommand>, HelloWorldPolicy>();

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var policy = scope.ServiceProvider
    .GetRequiredService<AbstractPolicyDescriptor<HelloWorldCommand>>();

// Creamos el evento para reaccionar
var userCreated = new UserCreatedEvent
{
    SomeProperty = "Prueba"
};
// Obtenemos las reaciones
var reactions = policy.GetReactions(userCreated);

var i = 90;

/*
 La poliza de proceso define el comportamiento a partir de un comando, y describe explicitamente los eventos 
capturados y sus reacciones hasta los comandos de reaccion
 */




/* Descriptor de proceso explicito
 * 
 * Para este descriptor, permite asociar una serie de pasos a un comando, segun el nivel de procesamiento que requerimos.
 * Permite especificar cual es el comando que escucharemos. 
 * Este comando puede asociarse una serie de eventos a traves del metodo fluido de Launch que permite definir un evento y asociarlo a comandos especificos.
 * Cada evento descrito, sera creado y sera transformado directamente en un comando de ese tipo y ejecutado directamente
 * 
 */

/*
 * Falta agregar verificacion para no repetir reacciones
 * Verificacion para no repetir eventpos
 * Revisar forma de devolver un comando con respuesta, cast generico, o en dado
 * caso inyectar el scheduler y agregarlo directamente y no devolverlo
 */