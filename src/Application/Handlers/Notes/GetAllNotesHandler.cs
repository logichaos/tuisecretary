using Mediator;
using TuiSecretary.Application.Queries.Notes;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Notes;

public class GetAllNotesHandler : IQueryHandler<GetAllNotesQuery, IEnumerable<Note>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllNotesHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async ValueTask<IEnumerable<Note>> Handle(GetAllNotesQuery query, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Notes.GetAllAsync(cancellationToken);
    }
}