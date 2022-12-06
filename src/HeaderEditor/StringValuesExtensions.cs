using Microsoft.Extensions.Primitives;

namespace HeaderEditor;

/// <summary>
/// Extension methods for instances of the <see cref="StringValues"/> class.
/// </summary>
public static class StringValuesExtensions
{
    /// <summary>
    /// Updates <param name="values">values</param> by performing <param name="func">func</param> to each value.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <param name="func">The function to apply to each value.</param>
    /// <returns>The updated <param name="values">values</param>.</returns>
    public static StringValues ToEach(this StringValues values, Func<string, string> func)
    {
        _ = func ?? throw new ArgumentNullException(nameof(func));

        return new StringValues(values.Select(func).ToArray());
    }

    /// <summary>
    /// Updates <param name="values">values</param> by performing <param name="func">func</param> to the value only if it's the only value.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <param name="func">The function to apply to the only value.</param>
    /// <returns>The updated <param name="values">values</param>.</returns>
    public static StringValues ToOnly(this StringValues values, Func<string, string> func)
    {
        _ = func ?? throw new ArgumentNullException(nameof(func));

        return values.Count == 1 ? new StringValues(func(values)) : values;
    }

    /// <summary>
    /// Updates <param name="values">values</param> by performing <param name="func">func</param> to only the first value.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <param name="func">The function to apply to the first value.</param>
    /// <returns>The updated <param name="values">values</param>.</returns>
    public static StringValues ToFirstOnly(this StringValues values, Func<string, string> func)
    {
        _ = func ?? throw new ArgumentNullException(nameof(func));

        return values == StringValues.Empty
            ? values
            : new StringValues(values.Select((v, i) => i == 0 ? func(v) : v).ToArray());
    }

    /// <summary>
    /// Updates <param name="values">values</param> by performing <param name="func">func</param> to only the last value.
    /// </summary>
    /// <param name="values">The string values.</param>
    /// <param name="func">The function to apply to the last value.</param>
    /// <returns>The updated <param name="values">values</param>.</returns>
    public static StringValues ToLastOnly(this StringValues values, Func<string, string> func)
    {
        _ = func ?? throw new ArgumentNullException(nameof(func));

        return values == StringValues.Empty
            ? values
            : new StringValues(values.Select((v, i) => i == values.Count - 1 ? func(v) : v).ToArray());
    }
}