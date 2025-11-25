using Microsoft.AspNetCore.Http;
using Shared.Exceptions;

namespace Application.CCC.ExceptionHandling.Handlers
{
    public abstract class ExceptionHandler
    {
        protected HttpResponse response;
        private Dictionary<Type, Func<Exception, Task>> exceptionAndHandlerDic;

        public ExceptionHandler()
        {
            exceptionAndHandlerDic = new()
            {
                { typeof(UnprocessableRequestException), HandleInvalidModelStateException },
                { typeof(NotFoundException), HandleNotFoundException },
                { typeof(BadRequestException), HandleBadRequestException },
                { typeof(ResourceNotFoundException), HandleResourceNotFoundException },
                { typeof(NoContentException), HandleNoContentException },
                { typeof(UnauthorizedException), HandleUnauthorizedException },
                { typeof(ForbiddenException), HandleForbiddenException },
                { typeof(InternalServerException), HandleServerException },
            };
        }

        public Task HandleException(Exception exception, HttpContext context)
        {
            response = context.Response;

            Type exceptionType = exception.GetType();
            Func<Exception, Task> handler = HandleUnknownException;

            Func<Exception, Task> findedHandler;
            exceptionAndHandlerDic.TryGetValue(exceptionType, out findedHandler);

            handler = findedHandler ?? handler;

            return handler.Invoke(exception);
        }

        protected abstract Task HandleResourceNotFoundException(Exception exception);
        protected abstract Task HandleNotFoundException(Exception exception);
        protected abstract Task HandleInvalidModelStateException(Exception exception);
        protected abstract Task HandleBadRequestException(Exception exception);
        protected abstract Task HandleUnknownException(Exception exception);
        protected abstract Task HandleUnauthorizedException(Exception exception);
        protected abstract Task HandleForbiddenException(Exception exception);
        protected abstract Task HandleNoContentException(Exception exception);
        protected abstract Task HandleServerException(Exception exception);
    }
}
