namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Evil : Neutral
{
    public abstract bool HasWon { get; }
    public abstract WinLose EndState { get; }

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralEvil;
    }
}