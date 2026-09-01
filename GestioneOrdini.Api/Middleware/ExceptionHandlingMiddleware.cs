using GestioneOrdini.Api.Errors;
using Microsoft.AspNetCore.Mvc;

namespace GestioneOrdini.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }

        catch (ResourceConflictException exception)
        {
            _logger.LogInformation(exception, "Risorsa in conflitto per {Method} {Path}", context.Request.Method, context.Request.Path);
            
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status409Conflict,
                "Risorsa in conflitto",
                exception.Message);
        }        
        catch (ResourceNotFoundException exception)
        {
            _logger.LogInformation(
                exception,
                "Risorsa non trovata per {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status404NotFound,
                "Risorsa non trovata",
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Errore inatteso per {Method} {Path}",
                context.Request.Method,
                context.Request.Path);

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Errore interno del server",
                "Si è verificato un errore inatteso.");
        }
    }

    private static Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path
        };
        
        problemDetails.Extensions["correlationId"] = context.TraceIdentifier;

        return context.Response.WriteAsJsonAsync(problemDetails);
    }
}