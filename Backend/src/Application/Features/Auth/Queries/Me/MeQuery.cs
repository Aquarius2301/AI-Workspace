using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record MeQuery(Guid UserId, CancellationToken CancellationToken) : IRequest<MeResult>;

public sealed record MeResult(
    Guid Id,
    string? AvatarUrl,
    string Name,
    string Email,
    LanguageDisplay Language
);

public sealed class MeQueryHandler : IRequestHandler<MeQuery, MeResult>
{
    private readonly IAppDbContext _context;

    public MeQueryHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<MeResult> Handle(MeQuery request, CancellationToken cancellationToken)
    {
        var user =
            await _context
                .Users.AsNoTracking()
                .Where(x => x.Id == request.UserId)
                .Select(x => new MeResult(
                    x.Id,
                    x.AvatarPicture != null ? x.AvatarPicture.Url : null,
                    x.Name,
                    x.Email,
                    x.Language
                ))
                .FirstOrDefaultAsync(cancellationToken)
            ?? throw new UnauthorizedException();

        return user;
    }
}
