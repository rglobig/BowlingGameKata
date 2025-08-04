namespace Code;

internal record Round
{
    public const int MaxPins = 10;

    public Round(int first, int second)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(first, nameof(first));
        ArgumentOutOfRangeException.ThrowIfNegative(second, nameof(second));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(first, MaxPins, nameof(first));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(second, MaxPins, nameof(second));

        First = first;
        Second = second;

        if (Sum > MaxPins)
            throw new ArgumentException($"Sum of first and second throws cannot be greater than {MaxPins}");
    }

    public int Second { get; }
    public int First { get; }
    public int Sum => First + Second;
    public bool IsSpare => Sum == MaxPins && First < MaxPins;
    public bool IsStrike => First == MaxPins;
}
