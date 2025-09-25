using Mediator;
using TuiSecretary.Application.Commands.Notes;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Notes;

public class DeleteNoteHandler : ICommandHandler<DeleteNoteCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteNoteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Unit> Handle(DeleteNoteCommand command, CancellationToken cancellationToken)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(command.Id, cancellationToken);
        
        if (note == null)
        {
            throw new InvalidOperationException($"Note with ID {command.Id} not found.");
        }

        await _unitOfWork.Notes.DeleteAsync(command.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Unit.Value;
    }
}