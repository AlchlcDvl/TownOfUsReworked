namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Evil : Neutral
{
    /// <summary>
    /// Gets a value indicating whether or not the layer has won.
    /// </summary>
    public virtual bool HasWon => false;

    /// <summary>
    /// Gets the related wing related end state for the layer.
    /// </summary>
    public virtual WinLose EndState { get; }

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