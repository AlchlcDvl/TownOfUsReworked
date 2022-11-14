using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.DiseasedMod
{
    public class Diseased : Modifier
    {
        public Diseased(PlayerControl player) : base(player)
        {
            Name = "Diseased";
            TaskText = () => "Your killers get a higher cooldown";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Diseased;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Diseased;
            AddToModifierHistory(ModifierType);
        }
    }
}