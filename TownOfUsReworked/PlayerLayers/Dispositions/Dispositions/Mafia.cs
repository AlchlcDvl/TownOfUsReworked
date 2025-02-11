namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Mafia : Disposition
{
    [ToggleOption]
    public static bool MafiaRoles = true;

    [ToggleOption]
    public static bool MafVent = false;

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Mafia : CustomColorManager.Disposition;
    public override string Symbol => "ω";
    public override LayerEnum Type => LayerEnum.Mafia;
    public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";

    protected override void Init()
    {
        base.Init();
        Player.GetRole().Faction = Faction.Neutral;
    }

    protected override void CheckWin()
    {
        if (!MafiaWin())
            return;

        WinState = WinLose.MafiaWins;
        Winner = true;
        CallRpc(CustomRPC.WinLose, WinLose.MafiaWins);
    }
}