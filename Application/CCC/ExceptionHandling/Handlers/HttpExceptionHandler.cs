using Application.CCC.ExceptionHandling.Models;
using Microsoft.AspNetCore.Http;
using Shared.Exceptions;
using System.Net;

namespace Application.CCC.ExceptionHandling.Handlers
{
    public class HttpExceptionHandler : ExceptionHandler
    {
        protected override Task HandleResourceNotFoundException(Exception exception)
        {
            int a = 12;
            var currentException = exception as ResourceNotFoundException;
            var problemDetail = new ProblemDetail(exception, HttpStatusCode.NotFound, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleNotFoundException(Exception exception)
        {
            var currentException = exception as NotFoundException;
            var problemDetail = new ProblemDetail(exception, HttpStatusCode.NotFound, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.NotFound;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleInvalidModelStateException(Exception exception)
        {
            var currentException = exception as UnprocessableRequestException;
            var problemDetail = new ValidationProblemDetail(exception).ToJson();
            response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleBadRequestException(Exception exception)
        {
            var currentException = exception as BadRequestException;
            var problemDetail = new ProblemDetail(exception, HttpStatusCode.BadRequest, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleUnknownException(Exception exception)
        {
            var currentException = exception as BadRequestException;

            var problemDetail = new ProblemDetail(exception, HttpStatusCode.BadRequest, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleServerException(Exception exception)
        {
            var currentException = exception as InternalServerException;

            var problemDetail = new ProblemDetail(exception, HttpStatusCode.InternalServerError, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleUnauthorizedException(Exception exception)
        {
            var currentException = exception as UnauthorizedAccessException;
            var problemDetail = new ProblemDetail(exception, HttpStatusCode.Unauthorized, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleForbiddenException(Exception exception)
        {
            var currentException = exception as ForbiddenException;
            var problemDetail = new ProblemDetail(exception, HttpStatusCode.Forbidden, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.Forbidden;
            return response.WriteAsync(problemDetail);
        }

        protected override Task HandleNoContentException(Exception exception)
        {
            var currentException = exception as NoContentException;
            var problemDetail = new ProblemDetail(exception, HttpStatusCode.NoContent, currentException?.Message).ToJson();
            response.StatusCode = (int)HttpStatusCode.NoContent;
            return response.WriteAsync(problemDetail);
        }
    }

}
