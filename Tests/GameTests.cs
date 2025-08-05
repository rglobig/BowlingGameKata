using Code;

namespace Tests;

public class GameTests
{
    private readonly Round _default = new(3, 3);
    private readonly ExtraSpareRound _extraSpareRound = new(5);
    private readonly ExtraStrikeRound _extraStrikeRound = new(3, 5);
    private readonly ExtraStrikeRound _extraStrikeRoundWithStrike = new(10, 0);
    private readonly IPlayableGame _game = Game.CreateNew();
    private readonly Round _spare = new(5, 5);
    private readonly Round _strike = new(10, 0);
    private readonly Round _zero = new(0, 0);

    [Fact]
    public void Play_Regular_Game()
    {
        var score = _game.PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_spare)
            .PlayRound(_default)
            .PlayRound(_strike)
            .PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_default)
            .Finish()
            .CompleteWithoutExtraRound()
            .GetScore();
        Assert.Equal(_default.Sum * 8 + (_spare.Sum + _default.First) + (_strike.Sum + _default.Sum), score);
    }

    [Fact]
    public void Game_Without_Rounds()
    {
        var extraRoundGame = _game.Finish();
        var completedGame = extraRoundGame.CompleteWithoutExtraRound();
        Assert.Equal(0, completedGame.GetScore());
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
        var completedGame = FinishGameAndCompleteWithoutExtraRound();
        Assert.Equal(0, completedGame.GetScore());
    }

    [Fact]
    public void Game_With_1_Pin_Hit_Each_Round()
    {
        PlayAllRounds(new Round(1, 0));
        var completedGame = FinishGameAndCompleteWithoutExtraRound();
        Assert.Equal(Game.MaxRounds * 1, completedGame.GetScore());
    }

    [Fact]
    public void Game_With_A_Spare()
    {
        _game.PlayRound(_spare);
        _game.PlayRound(_default);
        var completedGame = FinishGameAndCompleteWithoutExtraRound();
        Assert.Equal(_spare.Sum + _default.First + _default.Sum, completedGame.GetScore());
    }

    [Fact]
    public void Game_With_A_Strike()
    {
        _game.PlayRound(_strike);
        _game.PlayRound(_default);
        var completedGame = FinishGameAndCompleteWithoutExtraRound();
        Assert.Equal(_strike.Sum + _default.Sum + _default.Sum, completedGame.GetScore());
    }

    [Fact]
    public void Game_With_A_Spare_But_No_Round_After_Calculates_Score_Correctly()
    {
        var score = _game.PlayRound(_spare)
            .Finish()
            .CompleteWithoutExtraRound().GetScore();
        Assert.Equal(_spare.Sum, score);
    }

    [Fact]
    public void Game_With_A_Strike_But_No_Round_After_Calculates_Score_Correctly()
    {
        var score = _game.PlayRound(_strike)
            .Finish()
            .CompleteWithoutExtraRound().GetScore();
        Assert.Equal(_spare.Sum, score);
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_Allows_To_Play_Extra_Strike_Round()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_strike);
        var extraRoundGame = _game.Finish();
        var completedGame = extraRoundGame.PlayExtraStrikeRound(_extraStrikeRound);

        Assert.Equal(_strike.Sum + _extraStrikeRound.Sum * 2, completedGame.GetScore());
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_Doesnt_Allow_A_Extra_Spare_Round()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        var extraRoundGame = _game.PlayRound(_strike)
            .Finish();
        Assert.Throws<LastRoundIsNotSpareException>(() => extraRoundGame.PlayExtraSpareRound(_extraSpareRound));
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_Allows_Another_Extra_Strike_Round_But_This_Round_Doesnt_Allow_Another()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        var extraRoundGame = _game.PlayRound(_strike).Finish();
        extraRoundGame.PlayExtraStrikeRound(_extraStrikeRoundWithStrike);
        Assert.Throws<GameIsOverException>(() => extraRoundGame.PlayExtraStrikeRound(_extraStrikeRound));
    }

    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Allows_To_Play_Extra_Spare_Round()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        _game.PlayRound(_spare);
        var extraRoundGame = _game.Finish();
        var completedGame = extraRoundGame.PlayExtraSpareRound(_extraSpareRound);

        Assert.Equal(_spare.Sum + _extraSpareRound.Sum * 2, completedGame.GetScore());
    }

    [Fact]
    public void Game_Doesnt_Allow_Extra_Spare_Round_If_Last_Round_Is_Not_Spare()
    {
        PlayAllRounds(_zero);
        var extraRoundGame = _game.Finish();
        Assert.Throws<LastRoundIsNotSpareException>(() => extraRoundGame.PlayExtraSpareRound(_extraSpareRound));
    }

    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Allows_Another_Extra_Spare_Round_But_This_Round_Doesnt_Allow_Another()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        var extraRoundGame = _game.PlayRound(_spare).Finish();
        extraRoundGame.PlayExtraSpareRound(_extraSpareRound);
        Assert.Throws<GameIsOverException>(() => extraRoundGame.PlayExtraSpareRound(_extraSpareRound));
    }

    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Doesnt_Allow_A_Extra_Strike_Round()
    {
        PlayAllRoundsButLeaveLastRound(_zero);
        var extraRoundGame = _game.PlayRound(_spare).Finish();
        Assert.Throws<LastRoundIsNotStrikeException>(() => extraRoundGame.PlayExtraStrikeRound(_extraStrikeRound));
    }

    private void PlayAllRounds(Round round)
    {
        for (var i = 0; i < Game.MaxRounds; i++) _game.PlayRound(round);
    }

    private void PlayAllRoundsButLeaveLastRound(Round round)
    {
        for (var i = 0; i < Game.MaxRounds - 1; i++) _game.PlayRound(round);
    }

    private ICompletedGame FinishGameAndCompleteWithoutExtraRound()
    {
        var extraRoundGame = _game.Finish();
        var completedGame = extraRoundGame.CompleteWithoutExtraRound();
        return completedGame;
    }
}
