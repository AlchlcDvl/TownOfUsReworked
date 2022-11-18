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
            Color = CustomGameOptions.CustomModifierColors ? Colors.Professional : Colors.Modifier;
            ModifierType = ModifierEnum.Professional;
            LifeUsed = false;
            AddToModifierHistory(ModifierType);
        }
    }
}