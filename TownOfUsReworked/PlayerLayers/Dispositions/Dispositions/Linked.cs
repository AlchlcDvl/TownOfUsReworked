namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Linked : Disposition
{
    [ToggleOption]
    public static bool LinkedChat = true;

    [ToggleOption]
    public static bool LinkedRoles = true;

    public PlayerControl OtherLink { get; set; }

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Linked : CustomColorManager.Disposition;
    public override string Symbol => "Ψ";
    public override LayerEnum Type => LayerEnum.Linked;
    public override Func<string> Description => () => $"- Help {OtherLink.name} win";

    public override void OnMeetingEnd(MeetingHud __instance) => Player.GetRole().CurrentChannel = ChatChannel.Linked;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (OtherLink != player)
            return;

        name += $" {ColoredSymbol}";

        if (!LinkedRoles || revealed)
            return;

        var role = handler.CustomRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
        removeFromConsig = true;
    }
}