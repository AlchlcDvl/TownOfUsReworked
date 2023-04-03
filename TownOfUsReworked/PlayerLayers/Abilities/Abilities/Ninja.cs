using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Ninja : Ability
    {
        public Ninja(PlayerControl player) : base(player)
        {
            Name = "Ninja";
            TaskText = "- You do not lunge.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Ninja : Colors.Ability;
            Type = AbilityEnum.Ninja;
        }
    }
}