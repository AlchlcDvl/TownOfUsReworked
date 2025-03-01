namespace TownOfUsReworked.PlayerLayers.Dispositions;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Rivals : Disposition
{
    [ToggleOption]
    public static bool RivalsChat = true;

    [ToggleOption]
    public static bool RivalsRoles = true;

    public PlayerControl OtherRival { get; set; }
    public bool IsWinningRival =>  OtherRival.HasDied() && !Player.HasDied();

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Rivals : CustomColorManager.Disposition;
    public override string Symbol => "α";
    public override LayerEnum Type => LayerEnum.Rivals;
    public override Func<string> Description => () => OtherRival.HasDied() ? "- Live to the final 2" : $"- Get {OtherRival.name} killed";

    public override void OnMeetingEnd(MeetingHud __instance) => Player.GetRole().CurrentChannel = ChatChannel.Rivals;

    protected override void CheckWin()
    {
        if (!RivalsWin(Player))
            return;

        WinState = WinLose.RivalWins;
        Winner = true;
        CallRpc(CustomRPC.WinLose, WinLose.RivalWins, this);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (OtherRival != player)
            return;

        name += $" {ColoredSymbol}";

        if (!RivalsRoles || revealed)
            return;

        var role = handler.CustomRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
        removeFromConsig = true;
    }
}