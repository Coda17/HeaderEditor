namespace HeaderEditor.Test;

public partial class StringValuesExtensionsTests
{
    [Fact]
    public void ToEach_WithEmptyStringValues_ReturnsEmptyStringValues()
    {
        // act
        var result = StringValues.Empty.ToEach(v => $"{v}!");

        // assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void ToEach_WithSingleValue_ReturnsMutatedValue()
    {
        // arrange
        var value = $"{Guid.NewGuid()}";
        var values = new StringValues(value);

        // act
        var result = values.ToEach(v => $"{v.FirstOrDefault()}");

        // assert
        result.Should().NotBeEmpty().And.BeEquivalentTo(new StringValues($"{value.FirstOrDefault()}"));
    }

    [Fact]
    public void ToEach_WithMultipleValues_ReturnsMutatedValues()
    {
        // arrange
        var value1 = $"{Guid.NewGuid()}";
        var value2 = $"{Guid.NewGuid()}";
        var value3 = $"{Guid.NewGuid()}";
        var values = new StringValues(new[] { value1, value2, value3 });

        // act
        var result = values.ToEach(v => $"{v.LastOrDefault()}");

        // assert
        result.Should().NotBeEmpty().And.BeEquivalentTo(new StringValues(new[]
        {
            $"{value1.LastOrDefault()}",
            $"{value2.LastOrDefault()}",
            $"{value3.LastOrDefault()}"
        }));
    }

    [Fact]
    public void ToEach_WithFunctionThatThrowsException_BubblesUpException()
    {
        // arrange
        var value = $"{Guid.NewGuid()}";
        var values = new StringValues(value);

        // act
        Action act = () => values.ToEach(_ => throw new InvalidOperationException(value));

        // assert
        act.Should().Throw<InvalidOperationException>().WithMessage(value);
    }
}