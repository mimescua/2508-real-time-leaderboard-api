using Microsoft.AspNetCore.Diagnostics;
using SafeProjectName.Models;

namespace SafeProjectName.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
	private readonly ILogger<GlobalExceptionHandler> _logger;

	public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
	{
		_logger = logger;
	}

	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		_logger.LogError($"An error occurred while processing your request: {exception.Message}");

		httpContext.Response.ContentType = "application/json";

		var errorResponse = new ErrorResponse
		{
			Title = exception.Message,
			Message = exception.InnerException?.Message ?? "An unexpected error occurred",
			Instance = httpContext.Request.Path
		};

		errorResponse.StatusCode = exception switch
		{

			BadHttpRequestException => StatusCodes.Status400BadRequest,
			ArgumentException => StatusCodes.Status400BadRequest,
			InvalidOperationException => StatusCodes.Status409Conflict,
			InvalidDataException => StatusCodes.Status404NotFound,
			NullReferenceException => StatusCodes.Status404NotFound,
			KeyNotFoundException => StatusCodes.Status421MisdirectedRequest,
			_ => StatusCodes.Status500InternalServerError,
		};

		httpContext.Response.StatusCode = errorResponse.StatusCode;

		await httpContext.Response.WriteAsJsonAsync(errorResponse, cancellationToken);
		return true;
	}
}
