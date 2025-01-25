namespace TownOfUsReworked.PlayerLayers.Roles;

public class Crewmate : Crew
{
    public override LayerEnum Type => LayerEnum.Crewmate;
    public override Func<string> StartText => () => "Do Your Tasks";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }
}