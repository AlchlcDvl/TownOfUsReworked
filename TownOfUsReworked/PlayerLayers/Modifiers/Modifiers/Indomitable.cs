using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Indomitable : Modifier
    {
        public Indomitable(PlayerControl player) : base(player)
        {
            Name = "Indomitable";
            TaskText = "EEEK";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Indomitable : Colors.Modifier;
            ModifierType = ModifierEnum.Indomitable;
        }
    }
}