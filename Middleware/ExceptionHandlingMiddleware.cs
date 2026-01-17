using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AviaPartsAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
           RequestDelegate next,                    
           ILogger<ExceptionHandlingMiddleware> logger) 
        {
            this._next = next;      
            this._logger = logger;  
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Произошла ошибка при обработке запроса {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

         private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title) = exception switch
            {
                KeyNotFoundException _ => (HttpStatusCode.NotFound, "Ресурс не найден"),
                ArgumentException _ => (HttpStatusCode.BadRequest, "Некорректные параметры запроса"),
                _ => (HttpStatusCode.InternalServerError, "Внутренняя ошибка сервера")
            };

            var problemDetails = new ProblemDetails
            {
                Status = (int)statusCode,    
                Title = title,               
                Detail = exception.Message,  
                Instance = context.Request.Path, 
                Type = "about:blank"         
            };

            context.Response.StatusCode = (int)statusCode; 
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(problemDetails);

            await context.Response.WriteAsync(json);
        }
    }
}
