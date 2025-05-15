namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Neophyte : Neutral
{
    public override bool AffectedByLights => base.AffectedByLights && !NeutralNeophyteSettings.NnHaveImpVision;
    public override bool CanVent
    {
        get
        {
            var part = faction switch
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
                ? Faction.Compliance : Faction.Neutral);
        Members.Clear();
        Members.Add(PlayerId);
    }

    protected override void Deinit()
    {
        base.Deinit();
        Members.Clear();
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Members.Contains(player.PlayerId))
        {
            name += $" <#{SubFactionColor.ToHtmlStringRGBA()}>{SubFactionSymbol}</color>";

            if (GameModifiers.FactionSeeRoles && !revealed)
            {
                var role = handler.CustomRole;
                color = role.Color;
                name += $"\n{role}";
                revealed = true;
            }
            else
                color = SubFactionColor;
        }
    }
}