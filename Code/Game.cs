namespace Code;

internal class Game : IPlayableGame, IExtraRoundGame, ICompletedGame
{
    public const int MaxRounds = 10;

    private readonly List<Round> _rounds = [];

    private Game()
    {
    }

    public static IPlayableGame CreateNew()
    {
        return new Game();
    }

    public IPlayableGame PlayRound(Round round)
    {
        if (_rounds.Count >= MaxRounds)
            throw new GameIsOverException();

        _rounds.Add(round);
        return this;
    }

    public IExtraRoundGame Finish()
    {
        return this;
    }

    public int CalculateScore()
    {
        var score = 0;
        for (var i = 0; i < _rounds.Count; i++)
        {
            score += _rounds[i].Sum;

            if (IsLastRound(i)) continue;

            if (_rounds[i].IsStrike) score += NextRound(i).Sum;
            else if (_rounds[i].IsSpare) score += NextRound(i).First;
        }

        return score;
    }

    public ICompletedGame PlayExtraSpareRound(ExtraSpareRound extraSpareRound)
    {
        ValidateIfGameIsFinished();
        if (!GetLastRound().IsSpare) throw new LastRoundIsNotSpareException();

        _rounds.Add(extraSpareRound);
        return this;
    }

    public ICompletedGame CompleteWithoutExtraRound()
    {
        return this;
    }

    public ICompletedGame PlayExtraStrikeRound(ExtraStrikeRound extraStrikeRound)
    {
        ValidateIfGameIsFinished();
        if (!GetLastRound().IsStrike) throw new LastRoundIsNotStrikeException();

        _rounds.Add(extraStrikeRound);
        return this;
    }

    private void ValidateIfGameIsFinished()
    {
        switch (_rounds.Count)
        {
            case < MaxRounds:
                throw new GameIsNotOverException();
            case > MaxRounds:
                throw new GameIsOverException();
        }
    }

    private bool IsLastRound(int index)
    {
        return index + 1 == _rounds.Count;
    }
    
    private Round GetLastRound()
    {
        return _rounds[^1];
    }

    private Round NextRound(int index)
    {
        return _rounds[index + 1];
    }
}
