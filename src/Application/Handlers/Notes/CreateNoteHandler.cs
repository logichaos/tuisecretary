using Mediator;
using TuiSecretary.Application.Commands.Notes;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Notes;

public class CreateNoteHandler : ICommandHandler<CreateNoteCommand, Note>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateNoteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async ValueTask<Note> Handle(CreateNoteCommand command, CancellationToken cancellationToken)
    {
        var note = new Note(command.Title, command.Content, command.Tags);
        
        var createdNote = await _unitOfWork.Notes.AddAsync(note, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return createdNote;
    }
}