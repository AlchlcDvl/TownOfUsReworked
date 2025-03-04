namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Crewmate : Crew
{
    public override LayerEnum Type => LayerEnum.Crewmate;
    public override Func<string> StartText => () => "Do Your Tasks";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }
}