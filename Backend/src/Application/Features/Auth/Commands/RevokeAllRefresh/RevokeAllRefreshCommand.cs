using AIWorkspace.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record RevokeAllRefreshCommand(Guid UserId, CancellationToken CancellationToken)
    : IRequest;

public sealed class RevokeAllRefreshCommandHandler : IRequestHandler<RevokeAllRefreshCommand>
{
    private readonly IAppDbContext _context;

    public RevokeAllRefreshCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RevokeAllRefreshCommand request, CancellationToken cancellationToken)
    {
        // Bulk delete directly at DB level — no need to load entities into memory
        await _context
            .RefreshTokens.Where(x => x.UserId == request.UserId)
            .ExecuteDeleteAsync(cancellationToken);
    }
}
