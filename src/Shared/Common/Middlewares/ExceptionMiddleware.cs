using System.Net;
using System.Net.Mime;
using System.Text.Json;
using Common.ApiResponse;
using Common.Enum;
using Common.ErrorResult;
using Common.Extensions;
using Common.Middlewares.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Middlewares;

public class ExceptionMiddleware: IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = 500;

        httpContext.Response.ContentType = MediaTypeNames.Text.Html;

        await httpContext.Response.WriteAsync("<p>An error has occured - Custom here !</p>");

        return true;
    }
}