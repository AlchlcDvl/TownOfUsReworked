namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Torch : Ability
    {
        public Torch(PlayerControl player) : base(player)
        {
            Name = "Torch";
            TaskText = "- You can see in the dark";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Torch : Colors.Ability;
            AbilityType = AbilityEnum.Torch;
            Type = LayerEnum.Torch;
        }
    }
}