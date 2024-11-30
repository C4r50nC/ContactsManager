﻿using Serilog;

namespace ContactsManager.Ui.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IDiagnosticContext _diagnosticContext; // Currently unused

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IDiagnosticContext diagnosticContext)
        {
            _next = next;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                if (exception.InnerException != null)
                {
                    _logger.LogError("{ExceptionType} - {ExceptionMessage}", exception.InnerException.GetType().ToString(), exception.InnerException.Message);
                }
                else
                {
                    _logger.LogError("{ExceptionType} - {ExceptionMessage}", exception.GetType().ToString(), exception.Message);
                }

                // httpContext.Response.StatusCode = 500;
                // await httpContext.Response.WriteAsync("Error occurred - reported from ExceptionHandlingMiddleware");

                throw; // Added so errors can be caught by the built-in ExceptionHandler to redirect to Error page
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionHandlingMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}
