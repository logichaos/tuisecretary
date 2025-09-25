using Mediator;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Enums;

namespace TuiSecretary.Application.Commands.Tasks;

public record CreateTaskCommand(
    string Title,
    string Description = "",
    Priority Priority = Priority.Medium,
    DateTime? StartDate = null,
    DateTime? DueDate = null,
    TimeSpan? EstimatedDuration = null) : ICommand<WorkTask>;