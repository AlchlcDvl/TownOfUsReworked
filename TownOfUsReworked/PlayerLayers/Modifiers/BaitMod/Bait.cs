using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.BaitMod
{
    public class Bait : Modifier
    {
        public Bait(PlayerControl player) : base(player)
        {
            Name = "Bait";
            TaskText = () => "Killing you causes an instant self-report";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Bait;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Bait;
            AddToModifierHistory(ModifierType);
        }
    }
}