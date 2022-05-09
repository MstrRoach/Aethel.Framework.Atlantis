// See https://aka.ms/new-console-template for more information
using Aethel.Extensions.Application.Abstractions.Mediator;
using MediatR;

internal class PublishUserCreated : ICommand
{
    public string SomeProperty { get; set; }

    public PublishUserCreated(string someProperty)
    {
        SomeProperty = someProperty;
    }

    public Guid Id { get; } = Guid.NewGuid();
}