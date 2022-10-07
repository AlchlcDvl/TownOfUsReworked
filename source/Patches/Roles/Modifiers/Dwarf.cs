using TownOfUs.Extensions;
using UnityEngine;


namespace TownOfUs.Roles.Modifiers
{
    public class Dwarf : Modifier, IVisualAlteration
    {
        public Dwarf(PlayerControl player) : base(player)
        {
            var fastText = CustomGameOptions.DwarfSpeed != 1 ? " with zoomies!" : "!";
            Name = "Dwarf";
            TaskText = () => "Smol bean" + fastText;
            if (CustomGameOptions.CustomModifierColors) Color = Patches.Colors.Dwarf;
            else Color = Patches.Colors.Modifier;
            ModifierType = ModifierEnum.Dwarf;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.DwarfSpeed;
            appearance.SizeFactor = new Vector3(CustomGameOptions.DwarfScale, CustomGameOptions.DwarfScale, CustomGameOptions.DwarfScale);
            return true;
        }
    }
}
