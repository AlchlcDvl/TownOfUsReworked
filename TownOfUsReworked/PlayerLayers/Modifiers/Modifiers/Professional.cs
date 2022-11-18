using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Professional : Modifier
    {
        public bool LifeUsed;

        public Professional(PlayerControl player) : base(player)
        {
            Name = "Professional";
            TaskText = () => "You have an extra life when assassinating";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Professional;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Professional;
            LifeUsed = false;
            AddToModifierHistory(ModifierType);
        }
    }
}