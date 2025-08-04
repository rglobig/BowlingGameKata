namespace Code;

internal record ExtraSpareRound(int Roll) : Round(Roll, 0)
{
    public new int Sum => Roll;
}