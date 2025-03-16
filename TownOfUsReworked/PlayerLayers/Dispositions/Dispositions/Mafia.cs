namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Mafia)]
public sealed class Mafia : Disposition
{
    [ToggleOption]
    public static bool MafiaRoles = true;

    [ToggleOption]
    private static bool MafVent = false;

    public override UColor Color => ClientOptions.CustomDispColors ? CustomColorManager.Mafia : CustomColorManager.Disposition;
    public override string Symbol => "ω";
    public override LayerEnum Type => LayerEnum.Mafia;
    public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";
    public override bool CanVent => MafVent;

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

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (handler.CustomDisposition is not Mafia)
            return;

        name += $" {ColoredSymbol}";

        if (!MafiaRoles || revealed)
            return;

        var role = handler.CustomRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
        removeFromConsig = true;
    }
}