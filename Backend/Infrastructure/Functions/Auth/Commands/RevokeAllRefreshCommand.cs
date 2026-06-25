using DataAccess.UnitOfWork;
using MediatR;

namespace Infrastructure.Functions.Auth;

public sealed record RevokeAllRefreshCommand(
    Guid UserId,
    CancellationToken CancellationToken = default
) : IRequest;

public sealed class RevokeAllRefreshCommandHandler : IRequestHandler<RevokeAllRefreshCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeAllRefreshCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RevokeAllRefreshCommand request, CancellationToken cancellationToken)
    {
        var tokens = _unitOfWork
            .RefreshTokens.GetQuery()
            .Where(rt => rt.UserId == request.UserId)
            .ToList();

        if (tokens.Count > 0)
        {
            _unitOfWork.RefreshTokens.RemoveRange(tokens);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
