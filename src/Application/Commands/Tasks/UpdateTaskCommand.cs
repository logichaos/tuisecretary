using Mediator;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;

namespace TuiSecretary.Application.Commands.Tasks;

public record UpdateTaskCommand(
    Guid Id,
    string? Title = null,
    string? Description = null,
    Priority? Priority = null,
    TuiSecretary.Domain.Enums.TaskStatus? Status = null,
    DateTime? StartDate = null,
    DateTime? DueDate = null) : ICommand<WorkTask>;