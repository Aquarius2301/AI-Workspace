using DataAccess.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Auth;

public sealed record MeQuery(Guid UserId) : IRequest<MeResult>;

public sealed record MeResult(string Avatar, string Name, string Email);

public sealed class MeQueryHandler : IRequestHandler<MeQuery, MeResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public MeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MeResult> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork
            .Users.GetQuery()
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
        if (user is null)
            return new MeResult("", "", "");

        return new MeResult(user.AvatarUrl ?? "", user.Name, user.Email);
    }
}
