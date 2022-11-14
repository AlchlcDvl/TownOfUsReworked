using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.MultitaskerMod
{
    public class Multitasker : Ability
    {
        public Multitasker(PlayerControl player) : base(player)
        {
            Name = "Multitasker";
            TaskText = () => "Your task windows are transparent";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Multitasker;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Multitasker;
            AddToAbilityHistory(AbilityType);
        }
    }
}