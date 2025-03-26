namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Crew : Role
{
    protected override UColor MainColor => CustomColorManager.Crew;
    public override float VisionRange => CrewSettings.CrewVision;
    public override bool AffectedByLights => true;
    public override bool CanVent => CrewSettings.CrewVent == CrewVenting.Always || (CrewSettings.CrewVent == CrewVenting.OnTasksDone && TasksDone);
    protected override bool UseMainColor => ClientOptions.CustomCrewColors;

    protected override void Init()
    {
        base.Init();
        Faction = Faction.Crew;
        Objectives = () => CrewWinCon;
    }
}