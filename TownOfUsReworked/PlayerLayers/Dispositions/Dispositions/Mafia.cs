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
    public override LayerEnum Type { get; } = LayerEnum.Mafia;
    public override Func<string> Description => () => "- Eliminate anyone who opposes the Mafia";
    public override bool CanVent => MafVent;

    protected override void Init()
    {
        base.Init();
        Player.GetRole().Faction = Faction.Neutral;
    }

    protected override void CheckWin(List<byte> winnerIds)
    {
        if (AllPlayers().Any(x => !x.HasDied() && !x.Is<Mafia>()))
            return;

        WinState = WinLose.MafiaWins;
        winnerIds.Add(PlayerId);
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