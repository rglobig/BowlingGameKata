using Code;

namespace Tests;

public class RoundTests
{
    private readonly Round _spare = new(5, 5);
    private readonly Round _strike = new(10, 0);

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 4)]
    [InlineData(7, 3)]
    [InlineData(10, 0)]
    public void Sum_Of_Round_Is_Correct(int first, int second)
    {
        var round = new Round(first, second);
        Assert.Equal(first + second, round.Sum);
    }

    [Fact]
    public void Round_Is_Spare_If_First_And_Second_Roll_Sum_Is_MaxPins_And_First_Roll_Is_Less_Than_MaxPins()
    {
        Assert.True(_spare.IsSpare);
    }

    [Fact]
    public void Round_Is_Not_Spare_If_First_And_Second_Roll_Sum_Is_MaxPins_And_First_Roll_Is_MaxPins()
    {
        Assert.False(_strike.IsSpare);
    }

    [Fact]
    public void Round_Is_Not_Spare_If_First_And_Second_Roll_Sum_Is_Less_Than_MaxPins()
    {
        var round = new Round(3, 4);
        Assert.False(round.IsSpare);
    }

    [Fact]
    public void Round_Is_Strike_If_First_Roll_Is_MaxPins()
    {
        Assert.True(_strike.IsStrike);
    }

    [Fact]
    public void Round_Is_Not_Strike_If_First_Roll_Is_Less_Than_MaxPins()
    {
        var round = new Round(3, 4);
        Assert.False(round.IsStrike);
    }

    [Fact]
    public void First_Throw_Cant_Be_Negative_Throw_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Round(-1, 0));
    }

    [Fact]
    public void Second_Throw_Cant_Be_Negative_Throw_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Round(0, -1));
    }

    [Fact]
    public void First_Throw_Cant_Be_Greater_Than_MaxPins_Throw_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Round(Round.MaxPins + 1, 0));
    }

    [Fact]
    public void Second_Throw_Cant_Be_Greater_Than_MaxPins_Throw_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Round(0, Round.MaxPins + 1));
    }

    [Fact]
    public void Sum_Of_Round_Cant_Be_Greater_Than_MaxPins_Throw_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Round(Round.MaxPins - 1, 2));
        Assert.Equal(ex.Message, $"Sum of first and second throws cannot be greater than {Round.MaxPins}");
    }
}
