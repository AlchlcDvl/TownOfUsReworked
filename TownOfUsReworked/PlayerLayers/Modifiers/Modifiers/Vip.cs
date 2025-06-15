namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(Layer.Vip)]
public sealed class Vip : Modifier
{
    [ToggleOption]
    private static bool VipKnows = true;

    protected override UColor MainColor => CustomColorManager.Vip;
    public override Layer Type => Layer.Vip;
    public override string Description => "- Your death will alert everyone and will have an arrow pointing at your body";
    public override bool Hidden => !VipKnows && !Dead;

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        Flash(Color);
        var allArrows = LayerHandler.Handlers[LocalPlayer.PlayerId].AllArrows;

        if (!allArrows.TryGetValue(Player.PlayerId, out var arrow))
            allArrows[Player.PlayerId] = arrow = new(LocalPlayer, Player, Color);

        arrow.Update(Color);
    }
}