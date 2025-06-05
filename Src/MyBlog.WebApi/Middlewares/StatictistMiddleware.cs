using System.Diagnostics;

namespace MyBlog.WebApi.Middlewares;

public class StatictistMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var watch = new Stopwatch();
        watch.Start();
        Serilog.Log.Information(
            "Request started: {Method} {Path}",
            context.Request.Method,
            context.Request.Path
        );
        try
        {
            await next(context);
        }
        finally
        {
            watch.Stop();
            Serilog.Log.Information(
                "Request completed: {Method} {Path} in {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                watch.ElapsedMilliseconds
            );
        }
    }
}
