using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Errors;

public class NotFoundException (string message) : Exception (message);
public class InsufficientFundsException(string message) : Exception(message);

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext ctx,  Exception ex, CancellationToken tk)
    {
        var (status, title) = ex switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Recurso no encontrado"),
            InsufficientFundsException => (StatusCodes.Status422UnprocessableEntity, "Saldo insuficiente"),
            DbUpdateConcurrencyException => (StatusCodes.Status409Conflict, "Conflicto de concurrencia, reintentá la operación"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno")
        };
        if (status == 500) logger.LogError(ex, "Error no controlado");
        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = status == 500 ? null : ex.Message
        });
        return true;
    }
}
