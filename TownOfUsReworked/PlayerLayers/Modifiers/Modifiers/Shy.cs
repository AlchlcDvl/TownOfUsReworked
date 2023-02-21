using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Shy : Modifier
    {
        public Shy(PlayerControl player) : base(player)
        {
            Name = "Shy";
            TaskText = "Ummmmmm";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Shy : Colors.Modifier;
            ModifierType = ModifierEnum.Shy;
        }
    }
}