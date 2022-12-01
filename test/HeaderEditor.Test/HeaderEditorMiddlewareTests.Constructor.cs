namespace HeaderEditor.Test;

public partial class HeaderEditorMiddlewareTests
{
    [Fact]
    public void Constructor_WithNullRequestDelegate_ThrowsArgumentNullException()
    {
        // arrange
        RequestDelegate next = null!;
        var key = $"{Guid.NewGuid()}";
        string KeyMutation(string k) => k;
        StringValues ValuesMutation(StringValues values) => values;

        // act
        Action act = () => _ = new HeaderEditorMiddleware(next, key, KeyMutation, ValuesMutation);

        // assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("next");
    }

    [Fact]
    public void Constructor_WithNullKey_ThrowsArgumentNullException()
    {
        // arrange
        Task Next(HttpContext _) => Task.CompletedTask;
        string key = null!;
        string KeyMutation(string k) => k;
        StringValues ValuesMutation(StringValues values) => values;

        // act
        Action act = () => _ = new HeaderEditorMiddleware(Next, key, KeyMutation, ValuesMutation);

        // assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("key");
    }

    [Fact]
    public void Constructor_WithNullKeyMutation_ThrowsArgumentNullException()
    {
        // arrange
        Task Next(HttpContext _) => Task.CompletedTask;
        var key = $"{Guid.NewGuid()}";
        Func<string, string> keyMutation = null!;
        StringValues ValuesMutation(StringValues values) => values;

        // act
        Action act = () => _ = new HeaderEditorMiddleware(Next, key, keyMutation, ValuesMutation);

        // assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("keyMutation");
    }

    [Fact]
    public void Constructor_WithNullValuesMutation_ThrowsArgumentNullException()
    {
        // arrange
        Task Next(HttpContext _) => Task.CompletedTask;
        var key = $"{Guid.NewGuid()}";
        string KeyMutation(string k) => k;
        Func<StringValues, StringValues> valuesMutation = null!;

        // act
        Action act = () => _ = new HeaderEditorMiddleware(Next, key, KeyMutation, valuesMutation);

        // assert
        act.Should().Throw<ArgumentNullException>().WithParameterName("valuesMutation");
    }
}