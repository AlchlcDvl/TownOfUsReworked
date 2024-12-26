namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Rivals : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RivalsChat { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RivalsRoles { get; set; } = true;

    public PlayerControl OtherRival { get; set; }
    public bool IsWinningRival =>  OtherRival.HasDied() && !Player.HasDied();

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Rivals : CustomColorManager.Disposition;
    public override string Name => "Rivals";
    public override string Symbol => "α";
    public override LayerEnum Type => LayerEnum.Rivals;
    public override Func<string> Description => () => OtherRival.HasDied() ? "- Live to the final 2" : $"- Get {OtherRival.name} killed";

    public override void OnMeetingEnd(MeetingHud __instance) => Player.GetRole().CurrentChannel = ChatChannel.Rivals;

    public override void CheckWin()
    {
        if (RivalsWin(Player))
        {
            WinState = WinLose.RivalWins;
            Winner = true;
            CallRpc(CustomRPC.WinLose, WinLose.RivalWins, this);
        }
    }
}