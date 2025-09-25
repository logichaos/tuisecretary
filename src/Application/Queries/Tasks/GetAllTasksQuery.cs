using System.Diagnostics.CodeAnalysis;
using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Queries.Tasks;

[ExcludeFromCodeCoverage]
public record GetAllTasksQuery : IQuery<IEnumerable<WorkTask>>;