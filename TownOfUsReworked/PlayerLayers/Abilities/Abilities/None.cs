namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Abilityless : Ability
    {
        public Abilityless(PlayerControl player) : base(player)
        {
            Name = "None";
            Hidden = true;
        }
    }
}