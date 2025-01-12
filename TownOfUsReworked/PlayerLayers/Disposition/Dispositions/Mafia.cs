namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Mafia : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MafiaRoles { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool MafVent { get; set; } = false;

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Mafia : CustomColorManager.Disposition;
    public override string Symbol => "ω";
    public override LayerEnum Type => LayerEnum.Mafia;
    public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";

    public override void Init()
    {
        base.Init();
        Player.GetRole().Alignment = Player.GetRole().Alignment.GetNewAlignment(Faction.Neutral);
    }

    public override void CheckWin()
    {
        if (MafiaWin())
        {
            WinState = WinLose.MafiaWins;
            Winner = true;
            CallRpc(CustomRPC.WinLose, WinLose.MafiaWins);
        }
    }
}