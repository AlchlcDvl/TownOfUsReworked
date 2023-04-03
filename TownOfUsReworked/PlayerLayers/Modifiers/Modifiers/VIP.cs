using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class VIP : Modifier
    {
        public VIP(PlayerControl player) : base(player)
        {
            Name = "VIP";
            TaskText = "- Your death will alert everyone.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.VIP : Colors.Modifier;
            Type = ModifierEnum.VIP;
            Hidden = !CustomGameOptions.VIPKnows;
        }
    }
}