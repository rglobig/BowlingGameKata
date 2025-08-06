namespace Code;

internal interface IExtraRoundGame
{
    IExtraRoundGame PlayExtraRoll(Roll roll);
    ICompletedGame Complete();
}
