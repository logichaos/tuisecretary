using Mediator;
using TuiSecretary.Application.Queries.Tasks;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Tasks;

public class GetAllTasksHandler : IQueryHandler<GetAllTasksQuery, IEnumerable<WorkTask>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTasksHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<IEnumerable<WorkTask>> Handle(GetAllTasksQuery query, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Tasks.GetAllAsync(cancellationToken);
    }
}