using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Queries.Tasks;

public record GetAllTasksQuery : IQuery<IEnumerable<WorkTask>>;