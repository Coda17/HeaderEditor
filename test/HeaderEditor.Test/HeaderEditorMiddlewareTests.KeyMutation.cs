namespace HeaderEditor.Test;

public partial class HeaderEditorMiddlewareTests
{
    [Fact]
    internal async Task KeyMutation_WithNoMatchingHeader_DoesNothing()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";
        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => $"!{k}!");

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
    internal async Task KeyMutation_WithMatchingHeader_MutatesKeyButNotValue()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";
        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => $"{k}!");

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
            new($"{key}!", new[] { value })
        });
    }

    [Fact]
    internal async Task KeyMutation_WithMatchingHeaderWithMultipleValues_MutatesKeyButNotValues()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";
        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => $"{k}!");

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
            new($"{key}!", new[] { value, value2, value3 })
        });
    }

    [Fact]
    internal async Task KeyMutation_WithMatchingHeaderAndOtherHeaders_MutatesKeyOnlyForMatchingKey()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";
        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, k => k[0].ToString());

        using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        var client = host.GetTestClient();

        var key2 = $"{Guid.NewGuid()}";
        var key3 = $"{Guid.NewGuid()}";
        var value = $"{Guid.NewGuid()}";
        var value2 = $"{Guid.NewGuid()}";
        var value3 = $"{Guid.NewGuid()}";
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders =
            new KeyValuePair<string, IEnumerable<string>>[]
            {
                new(key, new[] { value }),
                new(key2, new[] { value2 }),
                new(key3, new[] { value3 })
            };

        using var request = CreateRequestMessage(requestHeaders);

        // act
        var response = await client.SendAsync(request);

        // assert
        response.StatusCode.Should().Be(ExpectedStatusCode);
        response.Headers.Should().BeEquivalentTo(new KeyValuePair<string, IEnumerable<string>>[]
        {
            new(key[0].ToString(), new[] { value }),
            new(key2, new[] { value2 }),
            new(key3, new[] { value3 })
        });
    }

    [Fact]
    internal async Task KeyMutation_WithMatchingHeaderAndMutatedKeyAlreadyExists_OverwritesMutatedKeyWithValue()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";
        var key2 = $"{Guid.NewGuid()}";
        void UseHeaderEditor(IApplicationBuilder app) => app.UseHeaderEditor(key, _ => key2);

        using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        var client = host.GetTestClient();

        var value = $"{Guid.NewGuid()}";
        var value2 = $"{Guid.NewGuid()}";
        IEnumerable<KeyValuePair<string, IEnumerable<string>>> requestHeaders =
            new KeyValuePair<string, IEnumerable<string>>[]
            {
                new(key, new[] { value }),
                new(key2, new[] { value2 })
            };

        using var request = CreateRequestMessage(requestHeaders);

        // act
        var response = await client.SendAsync(request);

        // assert
        response.StatusCode.Should().Be(ExpectedStatusCode);
        response.Headers.Should().BeEquivalentTo(new KeyValuePair<string, IEnumerable<string>>[]
        {
            new(key2, new[] { value })
        });
    }

    [Fact]
    internal async Task KeyMutation_WithExceptionInKeyMutationFunction_ThrowsOnHostStart()
    {
        // arrange
        var key = $"{Guid.NewGuid()}";

        void UseHeaderEditor(IApplicationBuilder app) =>
            app.UseHeaderEditor(key, _ => throw new InvalidOperationException(key));

        // act
        Func<Task> act = async () =>
        {
            using var host = await CreateHostBuilder(UseHeaderEditor).StartAsync();
        };

        // assert
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage(key);
    }
}