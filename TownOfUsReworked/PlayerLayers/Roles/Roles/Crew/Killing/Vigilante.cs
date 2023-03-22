using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Vigilante : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKilled;
        public bool KilledInno = false;
        public bool PreMeetingDie = false;
        public bool PostMeetingDie = false;
        public bool InnoMessage = false;
        public AbilityButton ShootButton;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public bool FirstRound = CustomGameOptions.RoundOneNoShot;

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            StartText = "Shoot The <color=#FF0000FF>Evildoers</color>";
            AbilitiesText = $"- You can shoot players.\n- You you shoot someone you are not supposed to, you will die to guilt.\n- You have {UsesLeft} bullets left.";
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
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.VigiKillCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}