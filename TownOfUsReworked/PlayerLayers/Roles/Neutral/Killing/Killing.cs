namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class NKilling : Neutral
{
    public override bool AffectedByLights => base.AffectedByLights && !NeutralKillingSettings.NkHaveImpVision;
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

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
        Faction = BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Killers
            ? Faction.Illuminati
            : (BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers == ComplianceType.Killers
                ? Faction.Compliance : Faction.Neutral);
    }
}