using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using System;

namespace TownOfUsReworked.PlayerLayers.Abilities
{
    public class ButtonBarry : Ability
    {
        public bool ButtonUsed;
        public AbilityButton ButtonButton;
        public DateTime LastButtoned;

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = "- You can call a button from anywhere.";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.ButtonBarry : Colors.Ability;
            AbilityType = AbilityEnum.ButtonBarry;
        }

        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastButtoned;
            var num = CustomGameOptions.ButtonCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}