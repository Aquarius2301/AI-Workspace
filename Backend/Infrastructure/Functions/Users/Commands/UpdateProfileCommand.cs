using BusinessObject.Entities;
using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Users;

public sealed record UpdateProfileCommand(Guid UserId, string? Name, string? AvatarUrl) : IRequest;

public sealed class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProfileCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user =
            await _unitOfWork
                .Users.GetQuery()
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        if (request.Name is not null)
            user.Name = request.Name;

        if (request.AvatarUrl is not null)
            user.AvatarUrl = request.AvatarUrl;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
