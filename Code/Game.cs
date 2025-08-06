namespace Code;

internal class Game : IPlayableGame, IExtraRoundGame, ICompletedGame
{
    public const int MaxRounds = 10;
    public const int MaxPins = 10;
    private const int MaxSpareExtraRolls = 1;
    private const int MaxStrikeExtraRolls = 2;

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
        for (var i = 0; i < _rounds.Count - 2; i++)
        {
            score += _rounds[i].Sum;

            if (_rounds[i].IsStrike)
            {
                score += GetRound(i + 1).First.PinsHit;
                score += GetRound(i + 1).IsStrike ? GetRound(i + 2).First.PinsHit : GetRound(i + 1).Second.PinsHit;
            }

            if (_rounds[i].IsSpare)
            {
                score += GetRound(i + 1).First.PinsHit;
            }
        }

        var beforeRound = GetRound(^2);
        score += beforeRound.Sum;
        if (beforeRound.IsSpare)
        {
            score += GetLastRound().First.PinsHit;
        }

        if (beforeRound.IsStrike)
        {
            score += GetLastRound().First.PinsHit;
            score += GetLastRound().IsStrike ? _extraRolls.First().PinsHit : GetLastRound().Second.PinsHit;
        }

        var lastRound = GetLastRound();

        score += lastRound.Sum;

        if (lastRound.IsStrike)
        {
            score += _extraRolls.Take(MaxStrikeExtraRolls).Sum(roll => roll.PinsHit);
        }

        if (lastRound.IsSpare)
        {
            score += _extraRolls.Take(MaxSpareExtraRolls).Sum(roll => roll.PinsHit);
        }

        return score;
    }

    public IExtraRoundGame PlayExtraRoll(Roll roll)
    {
        var lastRound = GetLastRound();

        if (lastRound.IsStrike && _extraRolls.Count < MaxStrikeExtraRolls)
        {
            _extraRolls.Add(roll);
        }
        else if (lastRound.IsSpare && _extraRolls.Count < MaxSpareExtraRolls)
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

        if (lastRound.IsStrike && _extraRolls.Count < MaxStrikeExtraRolls)
        {
            throw new GameIsNotOverException();
        }

        if (lastRound.IsSpare && _extraRolls.Count < MaxSpareExtraRolls)
        {
            throw new GameIsNotOverException();
        }

        return this;
    }

    private Round GetLastRound() => _rounds[^1];

    private Round GetRound(Index index)
    {
        if (index.Value < 0 || index.Value >= _rounds.Count)
            return new Round(new Roll(0), new Roll(0));
        return _rounds[index];
    }
}
