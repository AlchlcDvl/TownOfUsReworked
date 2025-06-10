namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class OKilling : Outcast
{
    public override bool AffectedByLights => base.AffectedByLights && !OutcastKillingSettings.NkHaveImpVision;
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
    public override Faction BaseFaction => BadGuysSettings.IlluminatiUnleashed && BadGuysSettings.IlluminatiMembers == IlluminatiType.Killers
        ? Faction.Illuminati
        : (BadGuysSettings.OrderOfCompliance && BadGuysSettings.ComplianceMembers == ComplianceType.Killers
            ? Faction.Compliance
            : (OutcastKillingSettings.WinSolo
                ? Faction.Outcast : ActualFaction));

    protected abstract Faction ActualFaction { get; }

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
    }
}