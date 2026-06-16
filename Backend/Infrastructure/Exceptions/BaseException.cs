using System.Net;

namespace Infrastructure.Exceptions
{
    public abstract class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; }

        protected BaseException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    // --- 400 Bad Request ---
    public class BadRequestException : BaseException
    {
        public BadRequestException(string message)
            : base(message, HttpStatusCode.BadRequest) { }
    }

    // --- 404 Not Found ---
    public class NotFoundException : BaseException
    {
        public NotFoundException(string message = "Not Found")
            : base(message, HttpStatusCode.NotFound) { }
    }

    // --- 401 Unauthorized ---
    public class UnauthorizedException : BaseException
    {
        public UnauthorizedException(string message = "Unauthorized")
            : base(message, HttpStatusCode.Unauthorized) { }
    }

    // --- 403 Forbidden ---
    public class ForbiddenException : BaseException
    {
        public ForbiddenException(string message = "Forbidden")
            : base(message, HttpStatusCode.Forbidden) { }
    }

    // --- 409 Conflict ---
    public class ConflictException : BaseException
    {
        public ConflictException(string message)
            : base(message, HttpStatusCode.Conflict) { }
    }

    // --- 422 Unprocessable Entity ---
    public class UnprocessableEntityException : BaseException
    {
        public UnprocessableEntityException(string message)
            : base(message, HttpStatusCode.UnprocessableEntity) { }
    }

    // --- 500 Internal Server Error (Cho các lỗi logic hệ thống chủ động báo) ---
    public class InternalServerErrorException : BaseException
    {
        public InternalServerErrorException(string message)
            : base(message, HttpStatusCode.InternalServerError) { }
    }
}
