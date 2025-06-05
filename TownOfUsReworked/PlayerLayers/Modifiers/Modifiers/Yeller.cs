namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Yeller : Modifier
{
    protected override UColor MainColor => CustomColorManager.Yeller;
    public override LayerEnum Type => LayerEnum.Yeller;
    public override Func<string> Description => () => "- Everyone knows where you are";

    private PlayerArrow Arrow { get; set; }

    protected override void Init()
    {
        if (!Local)
            Arrow = new(LocalPlayer, Player, Color);
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (Local)
            return;

        Arrow.Destroy();
        Arrow = null;
    }

    public override void OnRevive() => Init();
}