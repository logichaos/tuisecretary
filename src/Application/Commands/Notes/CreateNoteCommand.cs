using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Commands.Notes;

public record CreateNoteCommand(string Title, string Content, List<string>? Tags = null) : ICommand<Note>;