// See https://aka.ms/new-console-template for more information
using Aethel.Extensions.Application.Abstractions.Mediator;
using MediatR;

internal class SendWelcomeEmailCommand : ICommand
{
    public string SomeProperty { get; set; }

    public Guid Id { get; } = Guid.NewGuid();

    public SendWelcomeEmailCommand(string someProperty)
    {
        SomeProperty = someProperty;
    }
}