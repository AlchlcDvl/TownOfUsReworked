namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Evil : Neutral
{
    public abstract bool HasWon { get; }
    public abstract WinLose EndState { get; }

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Evil;
    }

    protected override void CheckWin()
    {
        if (NeutralEvilSettings.NeutralEvilsEndGame && HasWon)
        {
            WinState = EndState;
            Winner = true;
        }
    }
}