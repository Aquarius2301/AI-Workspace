using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;

namespace Infrastructure.Functions.Auth;

public sealed record RevokeSessionCommand(Guid UserId, string DeviceId) : IRequest;

public sealed class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeSessionCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RevokeSessionCommand request, CancellationToken cancellationToken)
    {
        var token = _unitOfWork
            .RefreshTokens.GetQuery()
            .FirstOrDefault(rt => rt.UserId == request.UserId && rt.DeviceId == request.DeviceId);

        if (token is null)
            throw new NotFoundException(ErrorCodes.SessionNotFound);

        _unitOfWork.RefreshTokens.Remove(token);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
