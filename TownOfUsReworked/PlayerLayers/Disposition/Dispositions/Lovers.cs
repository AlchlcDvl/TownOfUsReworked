namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Lovers : Disposition
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BothLoversDie { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LoversChat { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool LoversRoles { get; set; } = true;

    public PlayerControl OtherLover { get; set; }
    public bool LoversAlive => !Player.HasDied() && !OtherLover.HasDied();

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Lovers : CustomColorManager.Disposition;
    public override string Name => "Lovers";
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override Func<string> Description => () => $"- Live to the final 3 with {OtherLover.name}";

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (BothLoversDie && !OtherLover.HasDied() && !OtherLover.Is(Alignment.NeutralApoc))
            MurderPlayer(OtherLover);
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