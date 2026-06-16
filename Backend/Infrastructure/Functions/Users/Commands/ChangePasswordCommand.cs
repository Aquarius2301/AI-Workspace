using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using Infrastructure.Helpers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Users;

public sealed record ChangePasswordCommand(Guid UserId, string OldPassword, string NewPassword)
    : IRequest;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public ChangePasswordCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user =
            await _unitOfWork
                .Users.GetQuery()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException("User not found");

        if (!PasswordHelper.Verify(user.PasswordHash, request.OldPassword))
            throw new BadRequestException("Old password is incorrect");

        user.PasswordHash = PasswordHelper.Hash(request.NewPassword);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
