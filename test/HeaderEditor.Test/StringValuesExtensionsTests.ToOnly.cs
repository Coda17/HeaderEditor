namespace HeaderEditor.Test;

public partial class StringValuesExtensionsTests
{
    [Fact]
    public void ToOnly_WithEmptyStringValues_ReturnsEmptyStringValues()
    {
        // act
        var result = StringValues.Empty.ToOnly(k => $"{k}!");

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToOnly_WithSingleValue_ReturnsMutatedValue()
    {
        // arrange
        var value = $"{Guid.NewGuid()}";
        var values = new StringValues(value);

        // act
        var result = values.ToOnly(v => $"{v.FirstOrDefault()}");

        // assert
        result.Should().NotBeEmpty().And.BeEquivalentTo(new StringValues($"{value.FirstOrDefault()}"));
    }

    [Fact]
    public void ToOnly_WithMultipleValues_ReturnsUnchangedValues()
    {
        // arrange
        var value1 = $"{Guid.NewGuid()}";
        var value2 = $"{Guid.NewGuid()}";
        var value3 = $"{Guid.NewGuid()}";
        var values = new StringValues(new[] { value1, value2, value3 });

        // act
        var result = values.ToOnly(v => $"{v.LastOrDefault()}");

        // assert
        result.Should().NotBeEmpty().And.BeEquivalentTo(new StringValues(new[]
        {
            value1,
            value2,
            value3
        }));
    }

    [Fact]
    public void ToOnly_WithFunctionThatThrowsException_BubblesUpException()
    {
        // arrange
        var value = $"{Guid.NewGuid()}";
        var values = new StringValues(value);

        // act
        Action act = () => values.ToOnly(_ => throw new InvalidOperationException(value));

        // assert
        act.Should().Throw<InvalidOperationException>().WithMessage(value);
    }
}