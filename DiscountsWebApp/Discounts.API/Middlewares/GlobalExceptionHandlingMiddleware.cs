// Copyright (C) TBC Bank. All Rights Reserved.

using System.Net;
using System.Text.Json;
using Discounts.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Middlewares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex).ConfigureAwait(false);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred");

            var (statusCode, problemDetails) = exception switch
            {
                ValidationException validationEx => (
                    HttpStatusCode.BadRequest,
                    new ProblemDetails
                    {
                        Title = "Validation Error",
                        Status = (int)HttpStatusCode.BadRequest,
                        Detail = "One or more validation errors occurred.",
                        Extensions = { ["errors"] = validationEx.Errors }
                    }),

                NotFoundException notFoundEx => (
                    HttpStatusCode.NotFound,
                    new ProblemDetails
                    {
                        Title = "Not Found",
                        Status = (int)HttpStatusCode.NotFound,
                        Detail = notFoundEx.Message
                    }),

                ForbiddenAccessException forbiddenEx => (
                    HttpStatusCode.Forbidden,
                    new ProblemDetails
                    {
                        Title = "Forbidden",
                        Status = (int)HttpStatusCode.Forbidden,
                        Detail = forbiddenEx.Message
                    }),

                ConflictException conflictEx => (
                    HttpStatusCode.Conflict,
                    new ProblemDetails
                    {
                        Title = "Conflict",
                        Status = (int)HttpStatusCode.Conflict,
                        Detail = conflictEx.Message
                    }),

                _ => (
                    HttpStatusCode.InternalServerError,
                    new ProblemDetails
                    {
                        Title = "Internal Server Error",
                        Status = (int)HttpStatusCode.InternalServerError,
                        Detail = "An unexpected error occurred."
                    })
            };

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/json";

            var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            await context.Response.WriteAsync(json).ConfigureAwait(false);
        }
    }
}
