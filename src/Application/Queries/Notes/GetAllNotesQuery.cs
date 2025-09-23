using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Queries.Notes;

public record GetAllNotesQuery() : IQuery<IEnumerable<Note>>;