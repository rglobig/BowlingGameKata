namespace Code;

internal record StrikeRound() : Round(new StrikeRoll(), new ZeroRoll());

internal record Round
{
    public Round(Roll first, Roll second)
    {
        First = first;
        Second = second;

        if (PinsHit > Game.MaxPins)
            throw new ArgumentException($"Sum of first and second throws cannot be greater than {Game.MaxPins}");
    }

    public Roll First { get; }
    public Roll Second { get; }
    public int PinsHit => First.PinsHit + Second.PinsHit;
    public bool IsSpare => PinsHit == Game.MaxPins && First.PinsHit < Game.MaxPins;
    public bool IsStrike => First.PinsHit == Game.MaxPins;
}
