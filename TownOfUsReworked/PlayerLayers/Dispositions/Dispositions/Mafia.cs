namespace TownOfUsReworked.PlayerLayers.Dispositions;

[LayerHeaderOption(Layer.Mafia)]
public sealed class Mafia : Teamed
{
    [ToggleOption]
    private static bool MafiaRoles = true;

    [ToggleOption]
    private static bool MafVent = false;

    [ToggleOption]
    public static bool MafiaChat = false;

    protected override UColor MainColor => CustomColorManager.Mafia;
    public override string Symbol => "ω";
    public override Layer Type => Layer.Mafia;
    public override string Description => "- Eliminate anyone who opposes the Mafia";
    public override bool CanVent => MafVent;
    protected override bool RevealRole => MafiaRoles;
    public override ChatChannel Channel => ChatChannel.Mafia;
    public override bool CanChat => MafiaChat;

    public static readonly HashSet<byte> Mafias = [];

    public override void Init()
    {
        base.Init();
        Handler.CurrentFaction = Faction.Mafia;
        Mafias.Add(PlayerId);
    }

    protected override void CheckWin(HashSet<byte> winnerIds)
    {
        if (AllPlayers().Any(x => !x.HasDied() && !x.Is<Mafia>()))
            return;

        WinState = WinLose.MafiaWins;
        winnerIds.Add(PlayerId);
    }

    public override bool RoleCondition(PlayerControl player) => Mafias.Contains(player.PlayerId);
}