using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Volatile : Modifier
    {
        public Volatile(PlayerControl player) : base(player)
        {
            Name = "Volatile";
            TaskText = "- You experience a lot of hallucinations and lash out.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Volatile : Colors.Modifier;
            Type = ModifierEnum.Volatile;
            Hidden = !CustomGameOptions.VolatileKnows;
        }
    }
}