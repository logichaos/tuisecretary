using System.Diagnostics.CodeAnalysis;
using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Queries.Notes;

[ExcludeFromCodeCoverage]
public record GetNoteByIdQuery(Guid Id) : IQuery<Note?>;