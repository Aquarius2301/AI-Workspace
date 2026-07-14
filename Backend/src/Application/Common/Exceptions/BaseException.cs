using System.Net;

namespace AIWorkspace.Application.Common.Exceptions;

public abstract class BaseException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public new object? Data { get; }

    protected BaseException(string message, HttpStatusCode statusCode, object? data = null)
        : base(message)
    {
        StatusCode = statusCode;
        Data = data;
    }
}

// --- 400 Bad Request ---
public class BadRequestException : BaseException
{
    public BadRequestException(string message = ErrorCodes.BadRequest, object? data = null)
        : base(message, HttpStatusCode.BadRequest, data) { }
}

// --- 404 Not Found ---
public class NotFoundException : BaseException
{
    public NotFoundException(string message = ErrorCodes.NotFound, object? data = null)
        : base(message, HttpStatusCode.NotFound, data) { }
}

// --- 401 Unauthorized ---
public class UnauthorizedException : BaseException
{
    public UnauthorizedException(string message = ErrorCodes.Unauthorized, object? data = null)
        : base(message, HttpStatusCode.Unauthorized, data) { }
}

// --- 403 Forbidden ---
public class ForbiddenException : BaseException
{
    public ForbiddenException(string message = ErrorCodes.Forbidden, object? data = null)
        : base(message, HttpStatusCode.Forbidden, data) { }
}

// --- 409 Conflict ---
public class ConflictException : BaseException
{
    public ConflictException(string message = ErrorCodes.Conflict, object? data = null)
        : base(message, HttpStatusCode.Conflict, data) { }
}

// --- 422 Unprocessable Entity ---
public class UnprocessableEntityException : BaseException
{
    public UnprocessableEntityException(string message, object? data = null)
        : base(message, HttpStatusCode.UnprocessableEntity, data) { }
}

// --- 500 Internal Server Error ---
public class InternalServerErrorException : BaseException
{
    public InternalServerErrorException(
        string message = ErrorCodes.InternalServerError,
        object? data = null
    )
        : base(message, HttpStatusCode.InternalServerError, data) { }
}
