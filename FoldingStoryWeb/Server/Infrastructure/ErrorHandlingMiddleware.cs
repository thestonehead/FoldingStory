﻿namespace FoldingStoryWeb.Server.Infrastructure
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {

                await _next(context);
            } catch (Exception ex)
            {
                throw;
            }
        }
    }
}
