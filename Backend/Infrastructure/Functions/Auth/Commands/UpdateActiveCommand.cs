using DataAccess.UnitOfWork;
using Infrastructure.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Functions.Auth;

public sealed record UpdateActiveCommand(Guid CurrentUserId) : IRequest;

public sealed class UpdateActiveCommandHandler : IRequestHandler<UpdateActiveCommand>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateActiveCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateActiveCommand request, CancellationToken cancellationToken)
    {
        var user =
            await _unitOfWork
                .Users.GetQuery()
                .FirstOrDefaultAsync(u => u.Id == request.CurrentUserId, cancellationToken)
            ?? throw new NotFoundException(ErrorCodes.UserNotFound);

        user.LastActiveAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
