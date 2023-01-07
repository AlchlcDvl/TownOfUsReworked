using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Dwarf : Modifier, IVisualAlteration
    {
        public Dwarf(PlayerControl player) : base(player)
        {
            var fastText = CustomGameOptions.DwarfSpeed != 1 ? " with zoomies!" : "!";
            Name = "Dwarf";
            TaskText = "Smol bean" + fastText;
            Color = CustomGameOptions.CustomModifierColors ? Colors.Dwarf : Colors.Modifier;
            ModifierType = ModifierEnum.Dwarf;
            AddToModifierHistory(ModifierType);
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
