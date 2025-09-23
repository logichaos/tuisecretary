using Mediator;

namespace TuiSecretary.Application.Commands.Notes;

public record DeleteNoteCommand(Guid Id) : ICommand;