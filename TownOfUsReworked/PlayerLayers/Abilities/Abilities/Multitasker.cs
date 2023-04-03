using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class Multitasker : Ability
    {
        public Multitasker(PlayerControl player) : base(player)
        {
            Name = "Multitasker";
            TaskText = "- Your task windows are transparent.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.Multitasker : Colors.Ability;
            Type = AbilityEnum.Multitasker;
        }
    }
}