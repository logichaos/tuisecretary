using Mediator;
using TuiSecretary.Application.Commands.Tasks;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Tasks;

public class DeleteTaskHandler : ICommandHandler<DeleteTaskCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Unit> Handle(DeleteTaskCommand command, CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks.GetByIdAsync(command.Id, cancellationToken);
        
        if (task == null)
        {
            throw new InvalidOperationException($"Task with ID {command.Id} not found.");
        }

        await _unitOfWork.Tasks.DeleteAsync(command.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}