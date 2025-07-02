namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Crew : Role
{
    protected override UColor MainColor => CustomColorManager.Crew;
    protected override UColor LayerColor => CustomColorManager.Crew;
    public override bool CanVent => CrewSettings.CrewVent == CrewVenting.Always || (CrewSettings.CrewVent == CrewVenting.OnTasksDone && TasksDone);
    protected override bool UseMainColor => ClientOptions.CustomCrewColors;
    public override Faction BaseFaction => Faction.Crew;
    public override bool Local => base.Local || MimickedBy is { Local: true };
    public override bool Dead => base.Dead && MimickedBy is null or not { Dead: true };
    public override LayerHandler Handler
    {
        get => MimickedBy?.Handler ?? field;
        set;
    }

    public Retributionist MimickedBy;
}