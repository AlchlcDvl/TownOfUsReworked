namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Crewmate : Crew
{
    public override LayerEnum Type { get; } = LayerEnum.Crewmate;
    public override Func<string> StartText { get; } = () => "Do Your Tasks";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Utility;
    }
}