namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Bait : Modifier
{
    public override UColor Color => ClientGameOptions.CustomModColors ? CustomColorManager.Bait : CustomColorManager.Modifier;
    public override string Name => "Bait";
    public override LayerEnum Type => LayerEnum.Bait;
    public override Func<string> Description => () => "- Killing you causes the killer to report your body, albeit with a slight delay";
    public override bool Hidden => !CustomGameOptions.BaitKnows && !IsDead;

    public Bait() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }
}