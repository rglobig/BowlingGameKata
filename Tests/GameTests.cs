using Code;

namespace Tests;

public class GameTests
{
    private readonly Round _default = new(new Roll(3), new Roll(3));
    private readonly IPlayableGame _game = Game.CreateNew();
    private readonly Round _spare = new(new Roll(5), new Roll(5));
    private readonly StrikeRoll _strikeRoll = new();
    private readonly StrikeRound _strike = new();
    private readonly Round _zero = new(new ZeroRoll(), new ZeroRoll());
    private readonly Roll _defaultRoll = new(2);

    [Fact]
    public void Play_Regular_Game()
    {
        var score = _game
            .PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_spare)
            .PlayRound(_default)
            .PlayRound(_strike)
            .PlayRound(_default)
            .PlayRound(_default)
            .PlayRound(_strike)
            .Finish()
            .PlayExtraRoll(_defaultRoll)
            .PlayExtraRoll(_defaultRoll)
            .Complete()
            .CalculateScore();
        
        Assert.Equal(
            _default.PinsHit * 7 +
            (_spare.PinsHit + _default.First.PinsHit) +
            (_strike.PinsHit + _default.PinsHit) +
            (_strike.PinsHit + _defaultRoll.PinsHit + _defaultRoll.PinsHit),
            score);
    }

    [Fact]
    public void Play_Perfect_Game()
    {
        for (var i = 0; i < Game.MaxRounds; i++)
        {
            _game.PlayRound(_strike);
        }

        var extraRoundGame = _game.Finish();
        for (var i = 0; i < Game.ExtraScoreRollsForStrike; i++)
        {
            extraRoundGame.PlayExtraRoll(_strikeRoll);
        }

        var score = extraRoundGame.Complete()
            .CalculateScore();

        Assert.Equal(
            Game.MaxRounds * _strikeRoll.PinsHit +
            (Game.MaxRounds * _strikeRoll.PinsHit * Game.ExtraScoreRollsForStrike), score);
    }

    [Fact]
    public void Game_Without_Rounds_Throws_Exception()
    {
        Assert.Throws<GameIsNotOverException>(() => _game.Finish());
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
        var score = PlayAllRounds(_zero)
            .Finish()
            .Complete()
            .CalculateScore();
        
        Assert.Equal(0, score);
    }

    [Fact]
    public void Game_With_One_Pin_Hit_Each_Round()
    {
        var round = new Round(new Roll(1), new ZeroRoll());
        var score = PlayAllRounds(round)
            .Finish()
            .Complete()
            .CalculateScore();
        
        Assert.Equal(Game.MaxRounds * round.PinsHit, score);
    }

    [Fact]
    public void Game_With_A_Spare()
    {
        var score = PlayRounds(_zero, Game.MaxRounds - 2)
            .PlayRound(_spare)
            .PlayRound(_default)
            .Finish()
            .Complete()
            .CalculateScore();
        
        Assert.Equal(_spare.PinsHit + _default.First.PinsHit * Game.ExtraScoreRollsForSpare + _default.PinsHit, score);
    }

    [Fact]
    public void Game_With_A_Final_Spare()
    {
        var extraRoundGame = PlayRounds(_zero, Game.MaxRounds - 1)
            .PlayRound(_spare)
            .Finish();
        for (var i = 0; i < Game.ExtraScoreRollsForSpare; i++)
        {
            extraRoundGame.PlayExtraRoll(_defaultRoll);
        }

        var score = extraRoundGame.Complete()
            .CalculateScore();
        
        Assert.Equal(_strike.PinsHit + _defaultRoll.PinsHit * Game.ExtraScoreRollsForSpare, score);
    }
    
    [Fact]
    public void Game_With_A_Spare_As_Last_Round_Doesnt_Allow_More_Extra_Rolls()
    {
        var extraRoundGame = PlayAllRoundsButLeaveLastRound(_zero)
            .PlayRound(_spare)
            .Finish();
        for (var i = 0; i < Game.ExtraScoreRollsForSpare; i++)
        {
            extraRoundGame.PlayExtraRoll(_defaultRoll);
        }

        Assert.Throws<GameIsOverException>(() => extraRoundGame.PlayExtraRoll(_defaultRoll));
    }
    
    [Fact]
    public void Game_With_A_Spare_As_Last_Round_And_Not_Enough_Extra_Rolls_Throws_Exception()
    {
        var extraRoundGame = PlayRounds(_zero, Game.MaxRounds - 1)
            .PlayRound(_spare)
            .Finish();

        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        for (var i = 0; i < Game.ExtraScoreRollsForSpare - 1; i++)
        {
            extraRoundGame.PlayExtraRoll(_defaultRoll);
        }

        Assert.Throws<GameIsNotOverException>(() => extraRoundGame.Complete());
    }

    [Fact]
    public void Game_With_A_Strike()
    {
        var score = PlayRounds(_zero, Game.MaxRounds - 2)
            .PlayRound(_strike)
            .PlayRound(_default)
            .Finish()
            .Complete()
            .CalculateScore();
        
        Assert.Equal(_strike.PinsHit + _default.PinsHit * Game.ExtraScoreRollsForStrike, score);
    }

    [Fact]
    public void Game_With_A_Final_Strike()
    {
        var extraRoundGame = PlayRounds(_zero, Game.MaxRounds - 1)
            .PlayRound(_strike)
            .Finish();

        for (var i = 0; i < Game.ExtraScoreRollsForStrike; i++)
        {
            extraRoundGame.PlayExtraRoll(_defaultRoll);
        }

        var score = extraRoundGame.Complete()
            .CalculateScore();

        Assert.Equal(_strike.PinsHit + _defaultRoll.PinsHit * Game.ExtraScoreRollsForStrike, score);
    }

    [Fact]
    public void Game_With_No_Strike_Or_Spare_As_Last_Round_Dont_Allow_Extra_Roll()
    {
        var extraRoundGame = PlayAllRoundsButLeaveLastRound(_zero)
            .PlayRound(_default)
            .Finish();
        
        Assert.Throws<GameIsOverException>(() => extraRoundGame.PlayExtraRoll(_defaultRoll));
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_Dont_Allow_More_Extra_Rolls()
    {
        var extraRoundGame = PlayAllRoundsButLeaveLastRound(_zero)
            .PlayRound(_strike)
            .Finish();

        for (var i = 0; i < Game.ExtraScoreRollsForStrike; i++)
        {
            extraRoundGame.PlayExtraRoll(_defaultRoll);
        }

        Assert.Throws<GameIsOverException>(() => extraRoundGame.PlayExtraRoll(_defaultRoll));
    }

    [Fact]
    public void Game_With_A_Strike_As_Last_Round_And_Not_Enough_Extra_Rolls_Throws_Exception()
    {
        var extraRoundGame = PlayRounds(_zero, Game.MaxRounds - 1)
            .PlayRound(_strike)
            .Finish();

        for (var i = 0; i < Game.ExtraScoreRollsForStrike - 1; i++)
        {
            extraRoundGame.PlayExtraRoll(_defaultRoll);
        }

        Assert.Throws<GameIsNotOverException>(() => extraRoundGame.Complete());
    }

    private IPlayableGame PlayAllRounds(Round round)
    {
        return PlayRounds(round, Game.MaxRounds);
    }

    private IPlayableGame PlayAllRoundsButLeaveLastRound(Round round)
    {
        return PlayRounds(round, Game.MaxRounds - 1);
    }

    private IPlayableGame PlayRounds(Round round, int count)
    {
        for (var i = 0; i < count; i++) _game.PlayRound(round);
        return _game;
    }
}
