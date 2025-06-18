namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neophyte : Outcast
{
    public override bool AffectedByLights => base.AffectedByLights && !OutcastNeophyteSettings.NnHaveImpVision;
    public override bool CanVent
    {
        get
        {
            var part = Handler.CurrentFaction switch
            {
                Faction.Compliance => ComplianceSettings.ComplianceVent,
                Faction.Illuminati => IlluminatiSettings.IlluminatiVent,
                _ => true
            };
            return base.CanVent && part;
        }
    }
    public override Faction BaseFaction => BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Neophytes
        ? Faction.Illuminati
        : (BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers == ComplianceType.Neophytes
            ? Faction.Compliance : ActualFaction);
    public override Alignment Alignment => Alignment.Neophyte;
    public override bool SheriffSeesAsEvil => Sheriff.NeutNeophyteRed;

    protected abstract Faction ActualFaction { get; }

    public HashSet<byte> Members { get; } = [];

    public override void Init()
    {
        base.Init();
        Members.Clear();
        Members.Add(PlayerId);
    }

    protected override void Deinit() => Members.Clear();

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (!Members.Contains(player.PlayerId))
            return;

        if (GameModifiers.FactionSeeRoles && !revealed)
        {
            var role = handler.CurrentRole;
            color = role.Color;
            name += $"\n{role}";
            revealed = true;
        }
        else
            color = FactionColor;
    }
}