using HeaderEditor;
using Microsoft.Extensions.Primitives;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods to add header editor capabilities to an HTTP application pipeline.
/// </summary>
public static class HeaderEditorAppBuilderExtensions
{
    private static readonly Func<StringValues, StringValues> DefaultValuesMutation = values => values;

    /// <summary>
    /// Adds the header editor middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="key">The key of the header to mutate.</param>
    /// <param name="keyMutation">The key mutation.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseHeaderEditor(this IApplicationBuilder app, string key,
        Func<string, string> keyMutation) => app.UseHeaderEditor(key, keyMutation, DefaultValuesMutation);

    /// <summary>
    /// Adds the header editor middleware to the application pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="key">The key of the header to mutate.</param>
    /// <param name="keyMutation">The key mutation.</param>
    /// <param name="valuesMutation">The values mutation.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseHeaderEditor(this IApplicationBuilder app, string key,
        Func<string, string> keyMutation, Func<StringValues, StringValues> valuesMutation)
    {
        _ = app ?? throw new ArgumentNullException(nameof(app));
        _ = key ?? throw new ArgumentNullException(nameof(key));
        _ = keyMutation ?? throw new ArgumentNullException(nameof(keyMutation));
        _ = valuesMutation ?? throw new ArgumentNullException(nameof(valuesMutation));

        return app.UseMiddleware<HeaderEditorMiddleware>(key, keyMutation, valuesMutation);
    }
}