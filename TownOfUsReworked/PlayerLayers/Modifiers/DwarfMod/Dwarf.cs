using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.DwarfMod
{
    public class Dwarf : Modifier, IVisualAlteration
    {
        public Dwarf(PlayerControl player) : base(player)
        {
            var fastText = CustomGameOptions.DwarfSpeed != 1 ? " with zoomies!" : "!";
            Name = "Dwarf";
            TaskText = () => "Smol bean" + fastText;
            if (CustomGameOptions.CustomModifierColors) Color = Colors.Dwarf;
            else Color = Colors.Modifier;
            ModifierType = ModifierEnum.Dwarf;
            AddToModifierHistory(ModifierType);
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
