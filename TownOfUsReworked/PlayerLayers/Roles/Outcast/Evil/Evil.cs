namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Evil : Outcast
{
    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Evil;
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (!HasWon)
            return;

        if (OutcastEvilSettings.OutcastEvilsEndGame && !WinState.IsAny(EndState, WinLose.OutcastsWin))
            WinState = WinState is > WinLose.NobodyWins and < WinLose.OutcastsWin ? WinLose.OutcastsWin : EndState;

        winnerIds.Add(PlayerId);
    }
}