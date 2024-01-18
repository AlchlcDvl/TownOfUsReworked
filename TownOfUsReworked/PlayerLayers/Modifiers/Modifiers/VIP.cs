namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class VIP : Modifier
{
    public override UColor Color => ClientGameOptions.CustomModColors ? CustomColorManager.VIP : CustomColorManager.Modifier;
    public override string Name => "VIP";
    public override LayerEnum Type => LayerEnum.VIP;
    public override Func<string> Description => () => "- Your death will alert everyone and will have an arrow pointing at your body";
    public override bool Hidden => !CustomGameOptions.VIPKnows && !IsDead;

    public VIP(PlayerControl player) : base(player) {}
}