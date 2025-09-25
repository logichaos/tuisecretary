using Mediator;
using TuiSecretary.Application.Commands.Notes;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Notes;

public class UpdateNoteHandler : ICommandHandler<UpdateNoteCommand, Note>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateNoteHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<Note> Handle(UpdateNoteCommand command, CancellationToken cancellationToken)
    {
        var note = await _unitOfWork.Notes.GetByIdAsync(command.Id, cancellationToken);
        
        if (note == null)
        {
            throw new InvalidOperationException($"Note with ID {command.Id} not found.");
        }

        // Update fields if provided
        if (!string.IsNullOrEmpty(command.Title))
        {
            note.UpdateTitle(command.Title);
        }

        if (command.Content != null)
        {
            note.UpdateContent(command.Content);
        }

        if (command.Tags != null)
        {
            // Clear existing tags and add new ones
            foreach (var tag in note.Tags.ToList())
            {
                note.RemoveTag(tag);
            }
            
            foreach (var tag in command.Tags)
            {
                note.AddTag(tag);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return note;
    }
}