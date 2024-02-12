namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Ruthless : Ability
{
    public override UColor Color => ClientGameOptions.CustomAbColors ? CustomColorManager.Ruthless : CustomColorManager.Ability;
    public override string Name => "Ruthless";
    public override LayerEnum Type => LayerEnum.Ruthless;
    public override Func<string> Description => () => "- Your attacks cannot be stopped";
    public override bool Hidden => !CustomGameOptions.RuthlessKnows && !IsDead;

    public Ruthless() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }
}