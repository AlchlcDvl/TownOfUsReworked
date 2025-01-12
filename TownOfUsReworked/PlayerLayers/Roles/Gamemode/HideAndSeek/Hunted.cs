namespace TownOfUsReworked.PlayerLayers.Roles;

public class Hunted : HideAndSeek
{
    public override LayerEnum Type => LayerEnum.Hunted;
    public override Func<string> StartText => () => "Run, Hide And Do Tasks";
    public override UColor Color => CustomColorManager.Hunted;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without being hunted";
    }

    public override void CheckWin()
    {
        if (HuntedWin())
        {
            WinState = WinLose.HuntedWin;
            CallRpc(CustomRPC.WinLose, WinLose.HuntedWin);
        }
    }
}