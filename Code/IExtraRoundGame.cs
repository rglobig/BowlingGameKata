namespace Code;

internal interface IExtraRoundGame
{
    ICompletedGame PlayExtraStrikeRound(ExtraStrikeRound extraStrikeRound);
    ICompletedGame PlayExtraSpareRound(ExtraSpareRound extraSpareRound);
    ICompletedGame CompleteWithoutExtraRound();
}
