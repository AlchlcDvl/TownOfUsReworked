using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Giant : Modifier, IVisualAlteration
    {
        public Giant(PlayerControl player) : base(player)
        {
            var slowText = CustomGameOptions.GiantSpeed != 1 ? " and slow" : "";
            Name = "Giant";
            TaskText = $"- You are ginormous{slowText}.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Giant : Colors.Modifier;
            ModifierType = ModifierEnum.Giant;
            Type = LayerEnum.Giant;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor *= CustomGameOptions.GiantSpeed;
            appearance.SizeFactor *= CustomGameOptions.GiantScale;
            return true;
        }
    }
}