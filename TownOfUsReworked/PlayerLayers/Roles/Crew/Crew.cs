namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Crew : Role
{
    public override UColor Color => CustomColorManager.Crew;

    public override void Init()
    {
        base.Init();
        Faction = Faction.Crew;
        Objectives = () => CrewWinCon;
    }
}