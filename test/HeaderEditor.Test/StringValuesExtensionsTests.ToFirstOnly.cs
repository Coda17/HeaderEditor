namespace HeaderEditor.Test;

public partial class StringValuesExtensionsTests
{
    [Fact]
    public void ToFirstOnly_WithEmptyStringValues_ReturnsEmptyStringValues()
    {
        // act
        var result = StringValues.Empty.ToFirstOnly(k => $"{k}!");

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToFirstOnly_WithSingleValue_ReturnsMutatedValue()
    {
        // arrange
        var value = $"{Guid.NewGuid()}";
        var values = new StringValues(value);

        // act
        var result = values.ToFirstOnly(v => $"{v.FirstOrDefault()}");

        // assert
        result.Should().NotBeEmpty().And.BeEquivalentTo(new StringValues($"{value.FirstOrDefault()}"));
    }

    [Fact]
    public void ToFirstOnly_WithMultipleValues_ReturnsValuesWithOnlyFirstValueMutated()
    {
        // arrange
        var value1 = $"{Guid.NewGuid()}";
        var value2 = $"{Guid.NewGuid()}";
        var value3 = $"{Guid.NewGuid()}";
        var values = new StringValues(new[] { value1, value2, value3 });

        // act
        var result = values.ToFirstOnly(v => $"{v.LastOrDefault()}");

        // assert
        result.Should().NotBeEmpty().And.BeEquivalentTo(new StringValues(new[]
        {
            $"{value1.LastOrDefault()}",
            value2,
            value3
        }));
    }

    [Fact]
    public void ToFirstOnly_WithFunctionThatThrowsException_BubblesUpException()
    {
        // arrange
        var value = $"{Guid.NewGuid()}";
        var values = new StringValues(value);

        // act
        Action act = () => values.ToFirstOnly(_ => throw new InvalidOperationException(value));

        // assert
        act.Should().Throw<InvalidOperationException>().WithMessage(value);
    }
}