using DataAccess.UnitOfWork;
using Infrastructure.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Users;

public sealed record GetUsersQuery(
    PaginationRequest Pagination,
    CancellationToken CancellationToken = default
) : IRequest<PaginationResult<UserItem>>;

public sealed record UserItem(Guid Id, string Name, string Email);

public sealed class GetUsersQueryHandler
    : IRequestHandler<GetUsersQuery, PaginationResult<UserItem>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUsersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginationResult<UserItem>> Handle(
        GetUsersQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _unitOfWork.Users.ReadOnly();

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(u => u.Name)
            .Skip((request.Pagination.Page - 1) * request.Pagination.PageSize)
            .Take(request.Pagination.PageSize)
            .Select(u => new UserItem(u.Id, u.Name, u.Email))
            .ToListAsync(cancellationToken);

        return new PaginationResult<UserItem>(
            request.Pagination.Page,
            request.Pagination.PageSize,
            total,
            items
        );
    }
}
