namespace HeaderEditor.Test;

public partial class HeaderEditorMiddlewareTests
{
    [Fact]
    internal async Task ValuesMutation_WithNoMatchingHeader_DoesNothing()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";
        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => k, values => $"pre {values}");

        using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        var client = host.GetTestClient();

        var value = $"{Guid.NewGuid()}";
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders =
            new KeyValuePair<string, IEnumerable<string>>[]
            {
                new("no-match", new[] { value })
            };

        using var request = CreateRequestMessage(requestHeaders);

        // act
        var response = await client.SendAsync(request);

        // assert
        response.StatusCode.Should().Be(ExpectedStatusCode);
        response.Headers.Should().BeEquivalentTo(new KeyValuePair<string, IEnumerable<string>>[]
        {
            new("no-match", new[] { value })
        });
    }

    [Fact]
    internal async Task ValuesMutation_WithNoMatchingHeader_MutatesValuesButNotKey()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) =>
            app.UseHeaderEditor(key, k => k, values => values.Select(x => $"pre {x}").ToArray());

        using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        var client = host.GetTestClient();

        var value = $"{Guid.NewGuid()}";
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders =
            new KeyValuePair<string, IEnumerable<string>>[]
            {
                new(key, new[] { value })
            };

        using var request = CreateRequestMessage(requestHeaders);

        // act
        var response = await client.SendAsync(request);

        // assert
        response.StatusCode.Should().Be(ExpectedStatusCode);
        response.Headers.Should().BeEquivalentTo(new KeyValuePair<string, IEnumerable<string>>[]
        {
            new(key, new[] { $"pre {value}" })
        });
    }

    [Fact]
    internal async Task ValuesMutation_WithMatchingHeaderWithMultipleValues_MutatesValues()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => k,
            values => new StringValues(values.Select(x => $"{x}-post").ToArray()));

        using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        var client = host.GetTestClient();

        var value = $"{Guid.NewGuid()}";
        var value2 = $"{Guid.NewGuid()}";
        var value3 = $"{Guid.NewGuid()}";
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders =
            new KeyValuePair<string, IEnumerable<string>>[]
            {
                new(key, new[] { value, value2, value3 })
            };

        using var request = CreateRequestMessage(requestHeaders);

        // act
        var response = await client.SendAsync(request);

        // assert
        response.StatusCode.Should().Be(ExpectedStatusCode);
        response.Headers.Should().BeEquivalentTo(new KeyValuePair<string, IEnumerable<string>>[]
        {
            new(key, new[] { $"{value}-post", $"{value2}-post", $"{value3}-post" })
        });
    }

    [Fact]
    internal async Task ValuesMutation_WithExceptionInValuesMutation_BubbleUpExceptionFromMiddleware()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) =>
            app.UseHeaderEditor(key, k => k, values => throw new InvalidOperationException(values));

        using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        var client = host.GetTestClient();

        var value = $"{Guid.NewGuid()}";
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders =
            new KeyValuePair<string, IEnumerable<string>>[]
            {
                new(key, new[] { value })
            };


        // act
        Func<Task> act = async () =>
        {
            using var request = CreateRequestMessage(requestHeaders);
            _ = await client.SendAsync(request);
        };

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(value);
    }
}