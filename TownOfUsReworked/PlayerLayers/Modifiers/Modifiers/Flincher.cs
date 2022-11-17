using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Flincher : Modifier
    {
        public Flincher(PlayerControl player) : base(player)
        {
            Name = "Flincher";
            TaskText = () => "EEEK";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Flincher;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Flincher;
            AddToModifierHistory(ModifierType);
        }
    }
}