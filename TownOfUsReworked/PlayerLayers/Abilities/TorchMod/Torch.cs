using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Abilities.TorchMod
{
    public class Torch : Ability
    {
        public Torch(PlayerControl player) : base(player)
        {
            Name = "Torch";
            TaskText = () => "You can see in the dark";
            if (CustomGameOptions.CustomAbilityColors) Color = Colors.Torch;
            else Color = Colors.Ability;
            AbilityType = AbilityEnum.Torch;
            AddToAbilityHistory(AbilityType);
        }
    }
}