using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Modifiers.Modifiers
{
    public class Giant : Modifier, IVisualAlteration
    {
        public Giant(PlayerControl player) : base(player)
        {
            var slowText = CustomGameOptions.GiantSpeed != 1? " and slow!" : "!";
            Name = "Giant";
            TaskText = "You are ginormous" + slowText;
            Color = CustomGameOptions.CustomModifierColors ? Colors.Giant : Colors.Modifier;
            ModifierType = ModifierEnum.Giant;
            AddToModifierHistory(ModifierType);
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.GiantSpeed;
            appearance.SizeFactor = new Vector3(CustomGameOptions.GiantScale, CustomGameOptions.GiantScale, 1f);
            return true;
        }
    }
}