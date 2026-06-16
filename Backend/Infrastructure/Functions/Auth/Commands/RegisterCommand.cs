using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using MediatR;

namespace Infrastructure.Functions.Auth;

public sealed record RegisterCommand(string Name, string Email, string Password)
    : IRequest<RegisterResult>;

public sealed record RegisterResult(
    string Name,
    string Email,
    string AvatarUrl,
    DateTime CreatedAt
);

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResult>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<RegisterResult> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken
    )
    {
        var existingUser =
            _unitOfWork.Users.GetQuery().FirstOrDefault(u => u.Email == request.Email)
            ?? throw new ConflictException("Email already exists");

        var user = new BusinessObject.Entities.User
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Email = request.Email,
            PasswordHash = PasswordHelper.Hash(request.Password),
            AvatarUrl = null,
            CreatedAt = DateTime.UtcNow,
        };

        _unitOfWork.Users.Add(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new RegisterResult(user.Name, user.Email, user.AvatarUrl ?? "", user.CreatedAt);
    }
}
