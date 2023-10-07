namespace TownOfUsReworked.PlayerLayers.Roles;

public class Hunted : HideAndSeek
{
    public override string Name => "Hunted";
    public override LayerEnum Type => LayerEnum.Hunted;
    public override Func<string> StartText => () => "Run, Hide And Do Tasks";
    public override Color Color => Colors.Hunted;
    public override string FactionName => "Hide And Seek";

    public Hunted(PlayerControl player) : base(player)
    {
        Objectives = () => "- Finish your tasks before the others";
        Player.Data.SetImpostor(false);
    }
}