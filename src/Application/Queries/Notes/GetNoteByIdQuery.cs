using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Queries.Notes;

public record GetNoteByIdQuery(Guid Id) : IQuery<Note?>;