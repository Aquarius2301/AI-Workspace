using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Auth;

public sealed record GetSessionsQuery(Guid UserId, string? CurrentDeviceId)
    : IRequest<List<SessionResult>>;

public sealed record SessionResult(
    Guid Id,
    string? DeviceId,
    string? DeviceInfo,
    string? Browser,
    string? Os,
    DateTimeOffset CreatedAt,
    DateTimeOffset ExpiresAt,
    bool IsCurrent
);

public sealed class GetSessionsQueryHandler : IRequestHandler<GetSessionsQuery, List<SessionResult>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSessionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<SessionResult>> Handle(
        GetSessionsQuery request,
        CancellationToken cancellationToken
    )
    {
        var now = DateTimeOffset.UtcNow;

        var sessions = await _unitOfWork
            .RefreshTokens.GetQuery()
            .Where(rt => rt.UserId == request.UserId && rt.ExpiresAt > now)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);

        return sessions
            .Select(rt => new SessionResult(
                rt.Id,
                rt.DeviceId,
                rt.DeviceInfo,
                Browser: ParseBrowser(rt.DeviceInfo),
                Os: ParseOperatingSystem(rt.DeviceInfo),
                rt.CreatedAt,
                rt.ExpiresAt,
                IsCurrent: rt.DeviceId != null && rt.DeviceId == request.CurrentDeviceId
            ))
            .ToList();
    }

    private static string ParseBrowser(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return "Unknown";

        // Ordered by specificity to avoid false positives (e.g. Edge contains "Chrome")
        if (
            userAgent.Contains("Edg/")
            || userAgent.Contains("EdgA/")
            || userAgent.Contains("EdgiOS/")
        )
            return "Edge";
        if (userAgent.Contains("OPR/") || userAgent.Contains("Opera"))
            return "Opera";
        if (userAgent.Contains("Chrome/") && !userAgent.Contains("Chromium"))
            return "Chrome";
        if (userAgent.Contains("Chromium"))
            return "Chromium";
        if (userAgent.Contains("Firefox/") && !userAgent.Contains("Seamonkey"))
            return "Firefox";
        if (userAgent.Contains("Safari/") && !userAgent.Contains("Chrome"))
            return "Safari";

        return "Other";
    }

    private static string ParseOperatingSystem(string? userAgent)
    {
        if (string.IsNullOrWhiteSpace(userAgent))
            return "Unknown";

        if (userAgent.Contains("Windows NT 11"))
            return "Windows 11";
        if (userAgent.Contains("Windows NT 10"))
            return "Windows 10";
        if (userAgent.Contains("Windows NT 6.3"))
            return "Windows 8.1";
        if (userAgent.Contains("Windows NT 6.1"))
            return "Windows 7";
        if (userAgent.Contains("Windows"))
            return "Windows";
        if (userAgent.Contains("Mac OS X") || userAgent.Contains("macOS"))
            return "macOS";
        if (userAgent.Contains("Android"))
            return "Android";
        if (userAgent.Contains("iOS") || userAgent.Contains("iPhone") || userAgent.Contains("iPad"))
            return "iOS";
        if (userAgent.Contains("Linux") && !userAgent.Contains("Android"))
            return "Linux";

        return "Other";
    }
}
