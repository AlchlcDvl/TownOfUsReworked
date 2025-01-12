namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Yeller : Modifier
{
    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Yeller : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Yeller;
    public override Func<string> Description => () => "- Everyone knows where you are";

    public override void Init()
    {
        if (!Local)
            CustomPlayer.Local.GetRole().YellerArrows.TryAdd(PlayerId, new(CustomPlayer.Local, Player, Color));
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (!Local)
            CustomPlayer.Local.GetRole().DestroyArrowY(PlayerId);
    }

    public override void OnRevive() => Init();
}