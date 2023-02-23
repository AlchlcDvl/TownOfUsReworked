using UnityEngine;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers
{
    public class Dwarf : Modifier, IVisualAlteration
    {
        private string fastText = CustomGameOptions.DwarfSpeed != 1 ? " with speed" : "";

        public Dwarf(PlayerControl player) : base(player)
        {
            Name = "Dwarf";
            TaskText = $"- You are tiny{fastText}.";
            Color = CustomGameOptions.CustomModifierColors ? Colors.Dwarf : Colors.Modifier;
            ModifierType = ModifierEnum.Dwarf;
            ModifierDescription = "You are a Dwarf! You are small!";
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.DwarfSpeed;
            appearance.SizeFactor = new Vector3(CustomGameOptions.DwarfScale, CustomGameOptions.DwarfScale, 1f);
            return true;
        }
    }
}
