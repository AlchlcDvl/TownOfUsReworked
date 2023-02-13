using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Bait : Modifier
    {
        public Bait(PlayerControl player) : base(player)
        {
            Name = "Bait";
            TaskText = "Killing you causes an instant self-report";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Bait : Colors.Modifier;
            ModifierType = ModifierEnum.Bait;
            Hidden = !CustomGameOptions.BaitKnows;
        }
    }
}