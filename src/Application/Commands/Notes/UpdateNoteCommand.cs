using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Commands.Notes;

public record UpdateNoteCommand(Guid Id, string? Title = null, string? Content = null, List<string>? Tags = null) : ICommand<Note>;