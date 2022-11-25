using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Shy : Modifier
    {
        public Shy(PlayerControl player) : base(player)
        {
            Name = "Shy";
            TaskText = () => "Ummmmmm";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Shy : Colors.Modifier;
            ModifierType = ModifierEnum.Shy;
            AddToModifierHistory(ModifierType);
        }
    }
}