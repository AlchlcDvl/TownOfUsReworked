namespace TownOfUsReworked.PlayerLayers.Abilities;

public class Abilityless : Ability
{
    public override bool Hidden => true;

    public Abilityless(PlayerControl player) : base(player) {}
}