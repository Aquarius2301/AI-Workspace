using DataAccess.UnitOfWork;
using MediatR;

namespace Infrastructure.Functions.Auth;

public sealed record LogoutCommand(
    string RefreshToken,
    CancellationToken CancellationToken = default
) : IRequest;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public LogoutCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var storedToken = _unitOfWork
            .RefreshTokens.GetQuery()
            .FirstOrDefault(rt => rt.Token == request.RefreshToken);

        if (storedToken is not null)
        {
            _unitOfWork.RefreshTokens.Remove(storedToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
