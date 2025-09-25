using Mediator;
using TuiSecretary.Application.Commands.Tasks;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Tasks;

public class UpdateTaskHandler : ICommandHandler<UpdateTaskCommand, WorkTask>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<WorkTask> Handle(UpdateTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(command.Id, cancellationToken);
        
        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {command.Id} not found.");
        }

        // Update fields if provided
        if (!string.IsNullOrEmpty(command.Title))
        {
            task.UpdateTitle(command.Title);
        }

        if (command.Description != null)
        {
            task.UpdateDescription(command.Description);
        }

        if (command.Priority.HasValue)
        {
            task.UpdatePriority(command.Priority.Value);
        }

        if (command.Status.HasValue)
        {
            task.UpdateStatus(command.Status.Value);
        }

        if (command.StartDate.HasValue || command.DueDate.HasValue)
        {
            task.UpdateDates(command.StartDate, command.DueDate);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return task;
    }
}