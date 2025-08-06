namespace Code;

internal record StrikeRoll() : Roll(Game.MaxPins);

internal record Roll
{
    public int PinsHit { get; }

    public Roll(int pinsHit)
    {
        PinsHit = pinsHit;
        ArgumentOutOfRangeException.ThrowIfNegative(pinsHit, nameof(pinsHit));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pinsHit, Game.MaxPins, nameof(pinsHit));
    }
}
