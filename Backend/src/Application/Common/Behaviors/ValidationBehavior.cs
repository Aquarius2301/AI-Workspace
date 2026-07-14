using AIWorkspace.Application.Common.Exceptions;
using FluentValidation;
using MediatR;

namespace AIWorkspace.Application.Common.Behaviors;

/// <summary>
/// MediatR pipeline behavior that automatically validates requests using FluentValidation validators.
/// If validation fails, throws a <see cref="BadRequestException"/> with field-level error details.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken))
        );

        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            var errors = failures
                .Select(f => new { field = f.PropertyName, message = f.ErrorMessage })
                .Distinct()
                .ToList();

            throw new BadRequestException(ErrorCodes.BadRequest, errors);
        }

        return await next();
    }
}
