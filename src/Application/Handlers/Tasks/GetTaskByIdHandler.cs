using Mediator;
using TuiSecretary.Application.Queries.Tasks;
using TuiSecretary.Domain.Entities;
using TuiSecretary.Domain.Interfaces;

namespace TuiSecretary.Application.Handlers.Tasks;

public class GetTaskByIdHandler : IQueryHandler<GetTaskByIdQuery, WorkTask?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTaskByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async ValueTask<WorkTask?> Handle(GetTaskByIdQuery query, CancellationToken cancellationToken)
    {
        return await _unitOfWork.Tasks.GetByIdAsync(query.Id, cancellationToken);
    }
}