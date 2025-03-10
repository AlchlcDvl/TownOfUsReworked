namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Vip)]
public sealed class Vip : Modifier
{
    [ToggleOption]
    private static bool VipKnows = true;

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Vip : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Vip;
    public override Func<string> Description => () => "- Your death will alert everyone and will have an arrow pointing at your body";
    public override bool Hidden => !VipKnows && !Dead;

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        Flash(Player.GetRole().Color);
        var local = CustomPlayer.Local.GetRole();
        local.AllArrows.TryAdd(Player.PlayerId, new(CustomPlayer.Local, Player, Color));
        local.AllArrows[Player.PlayerId].Update(Color);
    }
}