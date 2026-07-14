using AIWorkspace.Application.Common;
using AIWorkspace.Application.Common.Exceptions;
using AIWorkspace.Application.Common.Services;
using AIWorkspace.Application.Helpers;
using AIWorkspace.Domain.Entities;
using AIWorkspace.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AIWorkspace.Application.Features.Auth;

public sealed record RegisterCommand(
    string Name,
    string Email,
    string Password,
    CancellationToken CancellationToken
) : IRequest;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IAppDbContext _context;

    public RegisterCommandHandler(IAppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var emailExists = await _context.Users.AnyAsync(
            x => x.Email == request.Email,
            cancellationToken
        );

        if (emailExists)
            throw new BadRequestException(
                ErrorCodes.BadRequest,
                new[] { new { field = "Email", message = ErrorCodes.EmailAlreadyExists } }
            );

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = PasswordHasherHelper.Hash(request.Password),
            Language = LanguageDisplay.Vi,
            CreatedAt = DateTimeOffset.UtcNow,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
