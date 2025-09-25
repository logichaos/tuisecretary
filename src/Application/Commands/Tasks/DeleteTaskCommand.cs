using Mediator;

namespace TuiSecretary.Application.Commands.Tasks;

public record DeleteTaskCommand(Guid Id) : ICommand;