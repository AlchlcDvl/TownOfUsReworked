using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Patches;
using System;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class ButtonBarry : Ability
    {
        public KillButton ButtonButton;
        public bool ButtonUsed;
        public DateTime StartingCooldown { get; set; }

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = "Call a button from anywhere!";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.ButtonBarry : Colors.Ability;
            AbilityType = AbilityEnum.ButtonBarry;
        }
        
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartingCooldown;
            var num = 10000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}