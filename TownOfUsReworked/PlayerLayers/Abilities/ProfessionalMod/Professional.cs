using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Abilities.ProfessionalMod
{
    public class Professional : Ability
    {
        public bool LifeUsed;

        public Professional(PlayerControl player) : base(player)
        {
            Name = "Professional";
            TaskText = () => "You have an extra life when assassinating";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Professional;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Professional;
            LifeUsed = false;
            AddToAbilityHistory(AbilityType);
        }
    }
}