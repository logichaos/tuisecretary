using Mediator;
using TuiSecretary.Application.Queries.Notes;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Notes;

public class GetNoteByIdHandler : IQueryHandler<GetNoteByIdQuery, Note?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetNoteByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async ValueTask<Note?> Handle(GetNoteByIdQuery query, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Notes.GetByIdAsync(query.Id, cancellationToken);
    }
}