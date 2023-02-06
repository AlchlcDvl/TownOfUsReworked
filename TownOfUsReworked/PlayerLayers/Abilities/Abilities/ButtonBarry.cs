using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using System;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Abilities.Abilities
{
    public class ButtonBarry : Ability
    {
        private KillButton _buttonButton;
        public bool ButtonUsed;
        public DateTime LastButtoned { get; set; }

        public ButtonBarry(PlayerControl player) : base(player)
        {
            Name = "Button Barry";
            TaskText = "Call a button from anywhere!";
            Color = CustomGameOptions.CustomAbilityColors ? Colors.ButtonBarry : Colors.Ability;
            AbilityType = AbilityEnum.ButtonBarry;
        }

        public KillButton ButtonButton
        {
            get => _buttonButton;
            set
            {
                _buttonButton = value;
                var role = Role.GetRole(Player);
                role?.AddToAbilityButtons(value, role);
            }
        }
        
        public float StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastButtoned;
            var num = CustomGameOptions.ButtonCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}