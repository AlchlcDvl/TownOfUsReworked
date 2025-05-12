namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class NKilling : Neutral
{
    public override bool AffectedByLights => base.AffectedByLights && !NeutralKillingSettings.NkHaveImpVision;

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