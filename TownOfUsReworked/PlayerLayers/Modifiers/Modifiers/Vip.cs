namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Vip)]
public sealed class Vip : Modifier
{
    [ToggleOption]
    private static bool VipKnows = true;

    protected override UColor MainColor => CustomColorManager.Vip;
    public override LayerEnum Type { get; } = LayerEnum.Vip;
    public override Func<string> Description => () => "- Your death will alert everyone and will have an arrow pointing at your body";
    public override bool Hidden => !VipKnows && !Dead;

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        Flash(Player.GetRole().Color);
        var local = CustomPlayer.Local.GetRole();
        local.AllArrows.TryAdd(Player.PlayerId, new(CustomPlayer.Local, Player, Color));
        local.AllArrows[Player.PlayerId].Update(Color);
    }
}