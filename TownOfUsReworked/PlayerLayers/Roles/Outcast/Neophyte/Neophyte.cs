namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neophyte : Outcast
{
    public override bool AffectedByLights => base.AffectedByLights && !OutcastNeophyteSettings.NnHaveImpVision;
    public override bool CanVent
    {
        get
        {
            var part = Faction switch
            {
                Faction.Compliance => ComplianceSettings.ComplianceVent,
                Faction.Illuminati => IlluminatiSettings.IlluminatiVent,
                _ => true
            };
            return base.CanVent && part;
        }
    }

    public HashSet<byte> Members { get; } = [];

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Neophyte;
        Faction = BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Neophytes
            ? Faction.Illuminati
            : (BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers == ComplianceType.Neophytes
                ? Faction.Compliance : Faction.Outcast);
        Members.Clear();
        Members.Add(PlayerId);
    }

    protected override void Deinit() => Members.Clear();

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Members.Contains(player.PlayerId))
        {
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
}