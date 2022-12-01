namespace HeaderEditor.Test;

public partial class HeaderEditorMiddlewareTests
{
    [Fact]
    internal async Task KeyAndValuesMutation_WithNoMatchingHeader_DoesNothing()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => $"{k}!",
            values => new StringValues(values.Select(x => $"{x}!").ToArray()));

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
    internal async Task KeyAndValuesMutation_WithMatchingHeaderWithSingleValue_MutatesKeyAndValue()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) =>
            app.UseHeaderEditor(key, k => $"{k}!", values => values.Select(x => $"pre {x}").ToArray());

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
            new($"{key}!", new[] { $"pre {value}" })
        });
    }

    [Fact]
    internal async Task KeyAndValuesMutation_WithMatchingHeaderWithMultipleValues_MutatesKeyAndValues()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => $"{k.FirstOrDefault()}",
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
            new($"{key.FirstOrDefault()}", new[] { $"{value}-post", $"{value2}-post", $"{value3}-post" })
        });
    }

    [Fact]
    internal async Task KeyAndValuesMutation_WithMatchingHeaderWithMultipleValuesAndExistingMutatedHeader_MutatesKeyAndValuesAndOverwritesExistingHeader()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => $"{k.FirstOrDefault()}",
            values => new StringValues(values.Select(x => $"{x}-post").ToArray()));

        using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        var client = host.GetTestClient();

        var value = $"{Guid.NewGuid()}";
        var value2 = $"{Guid.NewGuid()}";
        var value3 = $"{Guid.NewGuid()}";
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders =
            new KeyValuePair<string, IEnumerable<string>>[]
            {
                new(key, new[] { value, value2, value3 }),
                new($"{key.FirstOrDefault()}", new[] { $"{Guid.NewGuid()}" })
            };

        using var request = CreateRequestMessage(requestHeaders);

        // act
        var response = await client.SendAsync(request);

        // assert
        response.StatusCode.Should().Be(ExpectedStatusCode);
        response.Headers.Should().BeEquivalentTo(new KeyValuePair<string, IEnumerable<string>>[]
        {
            new($"{key.FirstOrDefault()}", new[] { $"{value}-post", $"{value2}-post", $"{value3}-post" })
        });
    }

    [Fact]
    internal async Task KeyAndValuesMutation_WithExceptionInKeyMutationFunction_ThrowsOnHostStart()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) =>
            app.UseHeaderEditor(key, _ => throw new InvalidOperationException(key), values => values);

        // act
        Func<Task> act = async () =>
        {
            using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        };

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(key);
    }
}