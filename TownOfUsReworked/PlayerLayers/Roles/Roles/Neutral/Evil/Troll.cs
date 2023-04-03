using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using System;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Troll : NeutralRole
    {
        public bool Killed;
        public DateTime LastInteracted;
        public AbilityButton InteractButton;
        public PlayerControl ClosestPlayer;
        public bool TrollWins;

        public Troll(PlayerControl player) : base(player)
        {
            Name = "Troll";
            StartText = "Troll Everyone With Your Death";
            AbilitiesText = "- You can interact with players.\n- Your interactions do nothing except spread infection and possibly kill you via touch sensitive roles.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Troll : Colors.Neutral;
            Type = RoleEnum.Troll;
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
        }

        public float InteractTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastInteracted;
            var num = CustomGameOptions.InteractCooldown * 1000f;
            var flag2 = num - (float)timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float)timespan.TotalMilliseconds) / 1000f;
        }
    }
}