using Mediator;
using TuiSecretary.Application.Commands.Tasks;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Tasks;

public class CreateTaskHandler : ICommandHandler<CreateTaskCommand, WorkTask>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<WorkTask> Handle(CreateTaskCommand command, CancellationToken cancellationToken)
    {
        var task = new WorkTask(
            command.Title,
            command.Description,
            command.Priority,
            command.StartDate,
            command.DueDate,
            command.EstimatedDuration);

        var createdTask = await _unitOfWork.Tasks.AddAsync(task, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return createdTask;
    }
}