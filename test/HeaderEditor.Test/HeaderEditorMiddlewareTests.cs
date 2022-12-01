namespace HeaderEditor.Test;

public partial class HeaderEditorMiddlewareTests
{
    private const HttpStatusCode ExpectedStatusCode = (HttpStatusCode)418;

    private static readonly Func<HttpContext, RequestDelegate, Task> RemoveHostHeader = (context, next) =>
    {
        _ = context.Request.Headers.Remove(HeaderNames.Host);
        return next(context);
    };

    private static readonly Func<HttpContext, RequestDelegate, Task> Endpoint = (context, next) =>
    {
        context.Response.StatusCode = (int)ExpectedStatusCode;
        foreach (var header in context.Request.Headers)
        {
            _ = context.Response.Headers.TryAdd(header.Key, header.Value);
        }

        return Task.CompletedTask;
    };

    private static IHostBuilder CreateHostBuilder(Action<IApplicationBuilder> useHeaderEditor) =>
        new HostBuilder()
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseTestServer()
                    .Configure(app =>
                    {
                        app.Use(RemoveHostHeader);
                        useHeaderEditor(app);
                        app.Use(Endpoint);
                    });
            });

    private static HttpRequestMessage CreateRequestMessage(
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/", UriKind.Relative));
        foreach (var header in headers)
        {
            request.Headers.Add(header.Key, header.Value);
        }

        return request;
    }
}