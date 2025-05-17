namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Evil : Neutral
{
    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Evil;
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!HasWon)
            return;

        if (NeutralEvilSettings.NeutralEvilsEndGame && !WinState.IsAny(EndState, WinLose.NeutralsWin))
            WinState = WinState is > WinLose.NobodyWins and < WinLose.NeutralsWin ? WinLose.NeutralsWin : EndState;

        winnerIds.Add(PlayerId);
    }
}