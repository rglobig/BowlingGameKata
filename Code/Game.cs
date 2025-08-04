namespace Code;

internal class Game
{
    public const int MaxRounds = 10;

    private readonly List<Round> _rounds = [];

    public void PlayRound(Round round)
    {
        if (_rounds.Count >= MaxRounds)
            throw new GameIsOverException();

        _rounds.Add(round);
    }

    public int GetScore()
    {
        var score = 0;
        for (var i = 0; i < _rounds.Count; i++)
        {
            score += _rounds[i].Sum;

            if (!HasNextRound(i)) continue;

            if (_rounds[i].IsStrike) score += NextRound(i).Sum;
            else if (_rounds[i].IsSpare) score += NextRound(i).First;
        }

        return score;
    }

    public void PlayExtraSpareRound(ExtraSpareRound extraSpareRound)
    {
        ValidateExtraRoundIsPlayable();
        if (!_rounds[^1].IsSpare) throw new LastRoundIsNotSpareException();

        _rounds.Add(extraSpareRound);
    }

    public void PlayExtraStrikeRound(ExtraStrikeRound extraStrikeRound)
    {
        ValidateExtraRoundIsPlayable();
        if (!_rounds[^1].IsStrike) throw new LastRoundIsNotStrikeException();

        _rounds.Add(extraStrikeRound);
    }

    private void ValidateExtraRoundIsPlayable()
    {
        switch (_rounds.Count)
        {
            case < MaxRounds:
                throw new GameIsNotOverException();
            case > MaxRounds:
                throw new GameIsOverException();
        }
    }

    private bool HasNextRound(int index)
    {
        return index + 1 < _rounds.Count;
    }

    private Round NextRound(int index)
    {
        return _rounds[index + 1];
    }
}