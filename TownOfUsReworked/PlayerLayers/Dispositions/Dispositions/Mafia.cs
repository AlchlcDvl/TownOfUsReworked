namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(LayerEnum.Mafia)]
public sealed class Mafia : Disposition
{
    [ToggleOption]
    public static bool MafiaRoles = true;

    [ToggleOption]
    private static bool MafVent = false;

    protected override UColor MainColor => CustomColorManager.Mafia;
    public override string Symbol => "ω";
    public override LayerEnum Type => LayerEnum.Mafia;
    public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";
    public override bool CanVent => MafVent;

    protected override void Init()
    {
        base.Init();
        Player.GetRole().Faction = Faction.Outcast;
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (AllPlayers().Any(x => !x.HasDied() && !x.Is<Mafia>()))
            return;

        WinState = WinLose.MafiaWins;
        winnerIds.Add(PlayerId);
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (handler.CurrentDisposition is not Mafia)
            return;

        name += $" {ColoredSymbol}";

        if (!MafiaRoles || revealed)
            return;

        var role = handler.CurrentRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
        removeFromConsig = true;
    }
}