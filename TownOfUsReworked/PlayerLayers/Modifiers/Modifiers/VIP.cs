using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class VIP : Modifier
    {
        public VIP(PlayerControl player) : base(player)
        {
            Name = "VIP";
            TaskText = () => "Your death will alert everyone";
            Color = CustomGameOptions.CustomModifierColors ? Colors.VIP : Colors.Modifier;
            ModifierType = ModifierEnum.Professional;
            AddToModifierHistory(ModifierType);
        }
    }
}