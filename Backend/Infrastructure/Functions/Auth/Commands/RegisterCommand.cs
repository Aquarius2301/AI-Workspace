using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using MediatR;

namespace Infrastructure.Functions.Auth;

public sealed record RegisterCommand(
    string Name,
    string Email,
    string Password,
    CancellationToken CancellationToken = default
) : IRequest;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        ValidatePassword(request.Password);

        var existingUser = _unitOfWork
            .Users.GetQuery()
            .FirstOrDefault(u => u.Email == request.Email);

        if (existingUser is not null)
            throw new ConflictException(ErrorCodes.EmailAlreadyExists);

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
    }

    private static void ValidatePassword(string password)
    {
        if (password.Length < 8)
            throw new BadRequestException(ErrorCodes.PasswordMinLength);

        if (!password.Any(char.IsUpper))
            throw new BadRequestException(ErrorCodes.PasswordRequireUpper);

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
            throw new BadRequestException(ErrorCodes.PasswordRequireSpecial);
    }
}
