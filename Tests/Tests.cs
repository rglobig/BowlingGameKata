using Code;

namespace Tests;

public class Tests
{
    private readonly Round _default = new(3, 3);
    private readonly ExtraSpareRound _extraSpareRound = new(5);
    private readonly ExtraStrikeRound _extraStrikeRound = new(3, 5);
    private readonly ExtraStrikeRound _extraStrikeRoundWithStrike = new(10, 0);
    private readonly Game _game = new();
    private readonly Round _spare = new(5, 5);
    private readonly Round _strike = new(10, 0);
    private readonly Round _zero = new(0, 0);

    [Fact]
    public void Game_Without_Rolls()
    {
        Assert.Equal(0, _game.GetScore());
    }

    [Fact]
    public void Game_Can_Only_Have_As_Much_As_MaxRounds_Otherwise_Throw_Exception()
    {
        PlayAllRounds(_zero);
        Assert.Throws<GameIsOverException>(() => _game.PlayRound(_zero));
    }

    [Fact]
    public void Game_WithoutAny_Hit_Pins()
    {
        PlayAllRounds(_zero);
        Assert.Equal(0, _game.GetScore());
    }

    [Fact]
    public void Game_With_1_Pin_Hit_Each_Round()
    {
        PlayAllRounds(new Round(1, 0));
        Assert.Equal(Game.MaxRounds * 1, _game.GetScore());
    }

    [Fact]
    public void Game_With_A_Spare()
    {
        _game.PlayRound(_spare);
        _game.PlayRound(_default);
        Assert.Equal(_spare.Sum + _default.First + _default.Sum, _game.GetScore());
    }

    [Fact]
    public void Game_Ends_After_Spare_Because_Player_Doesnt_Want_To_Play_Anymore()
    {
        _game.PlayRound(_spare);
        Assert.Equal(_spare.Sum, _game.GetScore());
    }

    [Fact]
    public void Game_With_A_Strike()
    {
        _game.PlayRound(_strike);
        _game.PlayRound(_default);
        Assert.Equal(_strike.Sum + _default.Sum + _default.Sum, _game.GetScore());
    }

    [Fact]
    public void Game_Ends_After_Strike_Because_Player_Doesnt_Want_To_Play_Anymore()
    {
        _game.PlayRound(_strike);
        Assert.Equal(_strike.Sum, _game.GetScore());
    }

    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Throws_Exception_If_Round_Contains_Two_Rolls()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_spare);
        Assert.Throws<GameIsOverException>(() => _game.PlayRound(_default));
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_Allows_Another_Full_Round()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_strike);
        _game.PlayExtraStrikeRound(_extraStrikeRound);
        Assert.Equal(_strike.Sum + _extraStrikeRound.Sum * 2, _game.GetScore());
    }

    [Fact]
    public void Game_Doesnt_Allow_Extra_Strike_Round_If_Game_Is_Not_Fully_Played()
    {
        Assert.Throws<GameIsNotOverException>(() => _game.PlayExtraStrikeRound(_extraStrikeRound));
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_Doesnt_Allow_A_Extra_Spare_Round()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_strike);
        Assert.Throws<LastRoundIsNotSpareException>(() => _game.PlayExtraSpareRound(_extraSpareRound));
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_Allows_Another_Full_Round_But_This_Round_Doesnt_Allow_Another()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_strike);
        _game.PlayExtraStrikeRound(_extraStrikeRoundWithStrike);
        Assert.Throws<GameIsOverException>(() => _game.PlayExtraStrikeRound(_extraStrikeRound));
    }

    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Allows_Another_Roll_But_This_Round_Doesnt_Allow_Another()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_spare);
        _game.PlayExtraSpareRound(_extraSpareRound);
        Assert.Throws<GameIsOverException>(() => _game.PlayExtraSpareRound(_extraSpareRound));
    }

    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Doesnt_Allow_A_Extra_Strike_Round()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_spare);
        Assert.Throws<LastRoundIsNotStrikeException>(() => _game.PlayExtraStrikeRound(_extraStrikeRound));
    }

    [Fact]
    public void Game_Doesnt_Allow_LastRoll_If_Game_Is_Not_Fully_Played()
    {
        Assert.Throws<GameIsNotOverException>(() => _game.PlayExtraSpareRound(_extraSpareRound));
    }

    [Fact]
    public void Game_Doesnt_Allow_LastRoll_If_Last_Round_Is_Not_Spare()
    {
        PlayAllRounds(_zero);
        Assert.Throws<LastRoundIsNotSpareException>(() => _game.PlayExtraSpareRound(_extraSpareRound));
    }

    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Allows_One_Last_Roll()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_spare);
        _game.PlayExtraSpareRound(_extraSpareRound);
        Assert.Equal(_spare.Sum + _extraSpareRound.Sum * 2, _game.GetScore());
    }

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

    private void PlayAllRounds(Round round)
    {
        for (var i = 0; i < Game.MaxRounds; i++) _game.PlayRound(round);
    }

    private void PlayAllRoundsButLeaveLastRound(Round round)
    {
        for (var i = 0; i < Game.MaxRounds - 1; i++) _game.PlayRound(round);
    }
}
