using Newtonsoft.Json;
using System.Net;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var originalResponseBodyStream = context.Response.Body;

        using (var memoryStream = new MemoryStream())
        {
            context.Response.Body = memoryStream;

            try
            {
                await _next(context); // Continuar con el siguiente middleware
                memoryStream.Seek(0, SeekOrigin.Begin);
                await memoryStream.CopyToAsync(originalResponseBodyStream); // Copiar al cliente la respuesta
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error durante el procesamiento de la solicitud.");
                await HandleExceptionAsync(context, ex); // Manejo de excepciones
            }
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

        var errorDetails = new
        {
            message = "Ocurrió un error interno en el servidor.",
            detail = exception.Message
        };

        // Enviar la respuesta de error como JSON
        await context.Response.WriteAsync(JsonConvert.SerializeObject(errorDetails));
    }
}
