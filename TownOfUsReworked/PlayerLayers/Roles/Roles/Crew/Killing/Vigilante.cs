using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Vigilante : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public bool KilledInno;
        public bool PreMeetingDie;
        public bool PostMeetingDie;
        public bool InnoMessage;
        public AbilityButton ShootButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool FirstRound = CustomGameOptions.RoundOneNoShot;

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            StartText = "Shoot The <color=#FF0000FF>Evildoers</color>";
            AbilitiesText = "- You can shoot players.\n- You you shoot someone you are not supposed to, you will die to guilt.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Vigilante : Colors.Crew;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Vigilante;
            RoleAlignment = RoleAlignment.CrewKill;
            AlignmentName = CK;
            InspectorResults = InspectorResults.UsesGuns;
            UsesLeft = CustomGameOptions.VigiBulletCount;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}