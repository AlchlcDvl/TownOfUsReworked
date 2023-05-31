namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Abilityless : Ability
    {
        public Abilityless(PlayerControl player) : base(player)
        {
            Name = "None";
            Hidden = true;
            Type = LayerEnum.None;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}