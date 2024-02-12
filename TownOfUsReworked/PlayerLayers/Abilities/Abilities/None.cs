namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Abilityless : Ability
{
    public override bool Hidden => true;

    public Abilityless() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        return this;
    }
}