using BusinessObject.Enums;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Auth;

public sealed record MeQuery(Guid UserId, CancellationToken CancellationToken = default)
    : IRequest<MeResult>;

public sealed record MeResult(string Avatar, string Name, string Email, string Language);

public sealed class MeQueryHandler : IRequestHandler<MeQuery, MeResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public MeQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<MeResult> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user =
            await _unitOfWork
                .Users.ReadOnly()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        return new MeResult(user.AvatarUrl ?? "", user.Name, user.Email, user.Language.ToString());
    }
}
