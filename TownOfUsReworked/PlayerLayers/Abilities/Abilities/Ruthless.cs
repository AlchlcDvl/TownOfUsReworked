namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Ruthless : Ability
    {
        public Ruthless(PlayerControl player) : base(player)
        {
            Name = "Ruthless";
            TaskText = () => "- Your attacks cannot be stopped";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Ruthless : Colors.Ability;
            AbilityType = AbilityEnum.Ruthless;
            Hidden = !CustomGameOptions.RuthlessKnows;
            Type = LayerEnum.Ruthless;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}