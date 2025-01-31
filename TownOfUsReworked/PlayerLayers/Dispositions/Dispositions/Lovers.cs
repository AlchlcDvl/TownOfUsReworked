namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Lovers : Disposition
{
    [ToggleOption]
    public static bool BothLoversDie = true;

    [ToggleOption]
    public static bool LoversChat = true;

    [ToggleOption]
    public static bool LoversRoles = true;

    [ToggleOption]
    public static bool ConvertLovers = true;

    public PlayerControl OtherLover { get; set; }
    public bool LoversAlive => !Player.HasDied() && !OtherLover.HasDied();

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Lovers : CustomColorManager.Disposition;
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override Func<string> Description => () => $"- Live to the final 3 with {OtherLover.name}";

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (BothLoversDie && !OtherLover.HasDied() && !OtherLover.Is(Alignment.Apocalypse))
            OtherLover.Suicide();
    }

    public override void OnMeetingEnd(MeetingHud __instance) => Player.GetRole().CurrentChannel = ChatChannel.Lovers;

    public override void CheckWin()
    {
        if (LoversWin(Player))
        {
            WinState = WinLose.LoveWins;
            Winner = true;
            OtherLover.GetDisposition().Winner = true;
            CallRpc(CustomRPC.WinLose, WinLose.LoveWins, this);
        }
    }
}