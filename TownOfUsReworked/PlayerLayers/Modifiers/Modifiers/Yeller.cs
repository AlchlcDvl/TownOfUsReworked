namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Yeller : Modifier
{
    protected override UColor MainColor => CustomColorManager.Yeller;
    public override LayerEnum Type => LayerEnum.Yeller;
    public override Func<string> Description => () => "- Everyone knows where you are";

    protected override void Init()
    {
        if (!Local)
            CustomPlayer.Local.GetRole().YellerArrows.TryAdd(PlayerId, new(CustomPlayer.Local, Player, Color));
    }

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (!Local)
            CustomPlayer.Local.GetRole().DestroyArrowY(PlayerId);
    }

    public override void OnRevive() => Init();
}