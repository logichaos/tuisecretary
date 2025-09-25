using System.Diagnostics.CodeAnalysis;
using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Queries.Tasks;

[ExcludeFromCodeCoverage]
public record GetTaskByIdQuery(Guid Id) : IQuery<WorkTask?>;