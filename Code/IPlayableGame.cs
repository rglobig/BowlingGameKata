namespace Code;

internal interface IPlayableGame
{
    IPlayableGame PlayRound(Round round);
    IExtraRoundGame Finish();
}
