using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
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
            AbilityDescription = "You are a Button Barry! You are paranoid of those around you and can call a meeting from anywhere at the cost of your vision!";
        }

        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastButtoned;
            var num = CustomGameOptions.ButtonCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}