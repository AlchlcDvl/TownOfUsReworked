namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Crewmate : CUtility
{
    public override LayerEnum Type => LayerEnum.Crewmate;
    public override Func<string> StartText { get; } = () => "Do Your Tasks";
}