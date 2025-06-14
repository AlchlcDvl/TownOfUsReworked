namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Yeller : Modifier
{
    protected override UColor MainColor => CustomColorManager.Yeller;
    public override LayerEnum Type => LayerEnum.Yeller;
    public override string Description => "- Everyone knows where you are";

    private PlayerArrow Arrow;

    public override void Init()
    {
        if (!Local)
            Arrow = new(LocalPlayer, Player, Color);
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (!Local)
            Arrow.Disable();
    }

    public override void OnRevive()
    {
        if (!Local)
            Arrow.Enable();
    }

    public override void EnteringLayer()
    {
        if (!Local && Arrow == null)
            Arrow = new(LocalPlayer, Player, Color);
    }
}