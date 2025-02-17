namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Evil : Neutral
{
    public abstract bool HasWon { get; }
    protected abstract WinLose EndState { get; }

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Evil;
    }

    protected override void CheckWin()
    {
        if (!NeutralEvilSettings.NeutralEvilsEndGame || !HasWon)
            return;

        WinState = EndState;
        Winner = true;
    }
}