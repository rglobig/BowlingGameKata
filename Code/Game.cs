namespace Code;

internal class Game : IPlayableGame, IExtraRoundGame, ICompletedGame
{
    public const int MaxRounds = 10;
    public const int MaxPins = 10;
    private const int ExtraScoreRollsForSpare = 1;
    private const int ExtraScoreRollsForStrike = 2;

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
            if (new Round(GetRoll(frameIndex), new ZeroRoll()).IsStrike)
            {
                score += new StrikeRound().PinsHit;
                for (var i = 1; i <= ExtraScoreRollsForStrike; i++)
                {
                    score += GetRoll(frameIndex + i).PinsHit;
                }

                frameIndex++;
            }
            else
            {
                var round = new Round(GetRoll(frameIndex), GetRoll(frameIndex + 1));
                score += round.PinsHit;
                if (round.IsSpare)
                {
                    for (var i = 1; i <= ExtraScoreRollsForSpare; i++)
                    {
                        score += GetRoll(frameIndex + 1 + i).PinsHit;
                    }
                }

                frameIndex += 2;
            }
        }

        return score;

        Roll GetRoll(int i)
        {
            if (i < 0 || i >= rolls.Count)
                return new ZeroRoll();
            return rolls[i];
        }
    }

    public IExtraRoundGame PlayExtraRoll(Roll roll)
    {
        var lastRound = GetLastRound();

        if (lastRound.IsStrike && _extraRolls.Count < ExtraScoreRollsForStrike)
        {
            _extraRolls.Add(roll);
        }
        else if (lastRound.IsSpare && _extraRolls.Count < ExtraScoreRollsForSpare)
        {
            _extraRolls.Add(roll);
        }
        else
        {
            throw new GameIsOverException();
        }

        return this;
    }

    public ICompletedGame Complete()
    {
        var lastRound = GetLastRound();

        if (lastRound.IsStrike && _extraRolls.Count < ExtraScoreRollsForStrike)
        {
            throw new GameIsNotOverException();
        }

        if (lastRound.IsSpare && _extraRolls.Count < ExtraScoreRollsForSpare)
        {
            throw new GameIsNotOverException();
        }

        return this;
    }

    private Round GetLastRound() => _rounds[^1];
}
