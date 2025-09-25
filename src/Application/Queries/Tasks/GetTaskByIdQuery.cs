using Mediator;
using TuiSecretary.Domain.Entities;

namespace TuiSecretary.Application.Queries.Tasks;

public record GetTaskByIdQuery(Guid Id) : IQuery<WorkTask?>;