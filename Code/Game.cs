namespace Code;

internal class Game : IPlayableGame, IExtraRoundGame, ICompletedGame
{
    public const int MaxRounds = 10;
    public const int MaxPins = 10;
    public const int ExtraScoreRollsForSpare = 1;
    public const int ExtraScoreRollsForStrike = 2;

    private readonly List<Round> _rounds = [];
    private readonly List<Roll> _extraRolls = [];

    private Game()
    {
    }

    public static IPlayableGame CreateNew() => new Game();

    public IPlayableGame PlayRound(Round round)
    {
        if (_rounds.Count >= MaxRounds)
            throw new GameIsOverException();

        _rounds.Add(round);
        return this;
    }

    public IExtraRoundGame Finish()
    {
        switch (_rounds.Count)
        {
            case < MaxRounds:
                throw new GameIsNotOverException();
            case > MaxRounds:
                throw new GameIsOverException();
        }

        return this;
    }

    public IExtraRoundGame PlayExtraRoll(Roll roll)
    {
        // ReSharper disable once InvertIf
        if (LastRoundIsStrikeAndExtraRollsAreNotFull() || 
            LastRoundIsSpareAndExtraRollsAreNotFull())
        {
            _extraRolls.Add(roll);
            return this;
        }

        throw new GameIsOverException();
    }

    public ICompletedGame Complete()
    {
        if (LastRoundIsStrikeAndExtraRollsAreNotFull() || 
            LastRoundIsSpareAndExtraRollsAreNotFull())
        {
            throw new GameIsNotOverException();
        }

        return this;
    }

    public int CalculateScore()
    {
        var score = 0;
        List<Roll> rolls = [];
        foreach (var round in _rounds)
        {
            rolls.Add(round.First);
            if (!round.IsStrike) rolls.Add(round.Second);
        }

        rolls.AddRange(_extraRolls);

        var frameIndex = 0;
        for (var frame = 0; frame < MaxRounds; frame++)
        {
            if (new Round(rolls[frameIndex], new ZeroRoll()).IsStrike)
            {
                score += new StrikeRound().PinsHit;
                for (var i = 1; i <= ExtraScoreRollsForStrike; i++)
                {
                    score += rolls[frameIndex + i].PinsHit;
                }

                frameIndex++;
            }
            else
            {
                var round = new Round(rolls[frameIndex], rolls[frameIndex + 1]);
                score += round.PinsHit;
                if (round.IsSpare)
                {
                    for (var i = 1; i <= ExtraScoreRollsForSpare; i++)
                    {
                        score += rolls[frameIndex + 1 + i].PinsHit;
                    }
                }

                frameIndex += 2;
            }
        }

        return score;
    }

    private bool LastRoundIsSpareAndExtraRollsAreNotFull()
    {
        return _rounds[^1].IsSpare && _extraRolls.Count < ExtraScoreRollsForSpare;
    }

    private bool LastRoundIsStrikeAndExtraRollsAreNotFull()
    {
        return _rounds[^1].IsStrike && _extraRolls.Count < ExtraScoreRollsForStrike;
    }
}
