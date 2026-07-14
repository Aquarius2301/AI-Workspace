using AIWorkspace.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record GetSessionsQuery(Guid UserId, string? CurrentDeviceId)
    : IRequest<List<SessionResult>>;

public sealed record SessionResult(
    string DeviceId,
    string? DeviceInfo,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    bool IsCurrent
);

public sealed class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, List<SessionResult>>
{
    private readonly IAppDbContext _context;

    public GetSessionsQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SessionResult>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var sessions = await _context
            .RefreshTokens.AsNoTracking()
            .Where(x => x.UserId == request.UserId && x.ExpiresAt > DateTimeOffset.UtcNow)
            .Select(x => new SessionResult(
                x.DeviceId!,
                x.DeviceInfo,
                x.CreatedAt,
                x.ExpiresAt,
                x.DeviceId == request.CurrentDeviceId
            ))
            .ToListAsync(cancellationToken);

        return sessions;
    }
}
