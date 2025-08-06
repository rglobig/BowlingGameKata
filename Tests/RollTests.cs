using Code;

namespace Tests;

public class RollTests
{
    [Fact]
    public void Roll_With_Zero_Pins_Hit_Is_Valid()
    {
        _ = new ZeroRoll();
    }

    [Fact]
    public void Roll_With_Max_Pins_Hit_Is_Valid()
    {
        _ = new Roll(Game.MaxPins);
    }

    [Fact]
    public void Roll_With_Negative_Pins_Hit_Throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Roll(-1));
    }

    [Fact]
    public void Roll_With_More_Than_Max_Pins_Hit_Throws_ArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Roll(Game.MaxPins + 1));
    }
}
