namespace TownOfUsReworked.PlayerLayers.Roles;

public class Hunted : HideAndSeek
{
    public override string Name => "Hunted";
    public override LayerEnum Type => LayerEnum.Hunted;
    public override Func<string> StartText => () => "Run, Hide And Do Tasks";
    public override UColor Color => CustomColorManager.Hunted;
    public override string FactionName => "Hide And Seek";

    public Hunted() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        Objectives = () => "- Finish your tasks before the others";
        Player.SetImpostor(false);
        return this;
    }
}