namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Lovers)]
public sealed class Lovers : Disposition
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

    public override UColor MainColor => CustomColorManager.Lovers;
    public override string Symbol => "♥";
    public override LayerEnum Type => LayerEnum.Lovers;
    public override Func<string> Description => () => $"- Live to the final 3 with {OtherLover.name}";

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (BothLoversDie && !OtherLover.HasDied() && !OtherLover.Is(Alignment.Apocalypse))
            OtherLover.Suicide();
    }

    public override void OnMeetingEnd(MeetingHud __instance) => Player.GetRole().CurrentChannel = ChatChannel.Lovers;

    protected override void CheckWin()
    {
        if (!LoversWin(Player))
            return;

        WinState = WinLose.LoveWins;
        Winner = true;
        OtherLover.GetDisposition().Winner = true;
        CallRpc(CustomRPC.WinLose, WinLose.LoveWins, this);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (OtherLover != player)
            return;

        name += $" {ColoredSymbol}";

        if (!LoversRoles || revealed)
            return;

        var role = handler.CustomRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
        removeFromConsig = true;
    }
}