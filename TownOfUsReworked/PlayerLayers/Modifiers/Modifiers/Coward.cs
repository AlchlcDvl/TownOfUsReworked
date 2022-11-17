using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Coward : Modifier
    {
        public Coward(PlayerControl player) : base(player)
        {
            Name = "Coward";
            TaskText = () => "You are too afraid to report bodies";
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Coward;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Coward;
            AddToModifierHistory(ModifierType);
        }
    }
}