using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace HeaderEditor;

/// <summary>
/// Middleware that edits headers.
/// </summary>
internal sealed class HeaderEditorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _key;
    private readonly string _mutatedKey;
    private readonly Func<StringValues, StringValues> _valuesMutation;

    /// <summary>
    /// Initializes a new instance of <see cref="HeaderEditorMiddleware"/>.
    /// </summary>
    /// <param name="next">The next middleware in the application pipeline.</param>
    /// <param name="key">The key of the header to mutate.</param>
    /// <param name="keyMutation">The mutation to perform on the header key.</param>
    /// <param name="valuesMutation">The mutation to perform on the header values.</param>
    /// <exception cref="ArgumentNullException">next, key, keyMutation, or valuesMutation.</exception>
    public HeaderEditorMiddleware(RequestDelegate next, string key, Func<string, string> keyMutation,
        Func<StringValues, StringValues> valuesMutation)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _key = key ?? throw new ArgumentNullException(nameof(key));
        _ = keyMutation ?? throw new ArgumentNullException(nameof(keyMutation));
        _valuesMutation = valuesMutation ?? throw new ArgumentNullException(nameof(valuesMutation));

        _mutatedKey = keyMutation(_key);
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var headers = context.Request.Headers;

        if (headers.TryGetValue(_key, out var existingValues))
        {
            var mutatedValues = _valuesMutation(existingValues);
            headers.Remove(_key);
            headers[_mutatedKey] = mutatedValues;
        }

        await _next(context).ConfigureAwait(false);
    }
}