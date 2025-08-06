using Code;

namespace Tests;

public class RoundTests
{
    private readonly Round _spare = new(new Roll(5), new Roll(5));
    private readonly Round _default = new(new Roll(3), new Roll(4));
    private readonly StrikeRound _strike = new();

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 4)]
    [InlineData(7, 3)]
    [InlineData(10, 0)]
    public void Sum_Of_Round_Is_Correct(int first, int second)
    {
        var round = new Round(new Roll(first), new Roll(second));
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
        Assert.False(_default.IsSpare);
    }

    [Fact]
    public void Round_Is_Strike_If_First_Roll_Is_MaxPins()
    {
        Assert.True(_strike.IsStrike);
    }

    [Fact]
    public void Round_Is_Not_Strike_If_First_Roll_Is_Less_Than_MaxPins()
    {
        Assert.False(_default.IsStrike);
    }

    [Fact]
    public void Sum_Of_Round_Cant_Be_Greater_Than_MaxPins_Throw_ArgumentOutOfRangeException()
    {
        var ex = Assert.Throws<ArgumentException>(() => new Round(new Roll(Game.MaxPins - 1), new Roll(2)));
        Assert.Equal(ex.Message, $"Sum of first and second throws cannot be greater than {Game.MaxPins}");
    }
}
