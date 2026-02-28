using System.Net;
using FluentValidation;
using PeladaPay.API.Contracts;
using PeladaPay.Application.Exceptions;
using PeladaPay.Domain.Exceptions;

namespace PeladaPay.API.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await context.Response.WriteAsJsonAsync(new ApiValidationErrorResponse(
                context.Response.StatusCode,
                "Erro de validação",
                ex.Errors.Select(x => x.ErrorMessage).ToList()));
        }
        catch (NotFoundException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            await context.Response.WriteAsJsonAsync(new ApiErrorResponse(context.Response.StatusCode, ex.Message));
        }
        catch (AsaasIntegrationException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            await context.Response.WriteAsJsonAsync(new ApiErrorResponse(context.Response.StatusCode, ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            await context.Response.WriteAsJsonAsync(new ApiErrorResponse(context.Response.StatusCode, ex.Message));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled error");
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            await context.Response.WriteAsJsonAsync(new ApiErrorResponse(context.Response.StatusCode, ex.Message));
        }
    }
}
