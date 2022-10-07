using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Modifiers
{
    public class Giant : Modifier, IVisualAlteration
    {
        public Giant(PlayerControl player) : base(player)
        {
            var slowText = CustomGameOptions.GiantSpeed != 1? " and slow!" : "!";
            Name = "Giant";
            TaskText = () => "You are ginormous" + slowText;
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Giant;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Giant;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.GiantSpeed;
            appearance.SizeFactor = new Vector3(CustomGameOptions.GiantScale, CustomGameOptions.GiantScale, CustomGameOptions.GiantScale);
            return true;
        }
    }
}