using System;
using TownOfUs.CrewmateRoles.TimeLordMod;
using TMPro;

namespace TownOfUs.Roles
{
    public class TimeLord : Role
    {
        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;
        public TimeLord(PlayerControl player) : base(player)
        {
            Name = "Time Lord";
            ImpostorText = () => "Rewind Time";
            TaskText = () => "Rewind Time!";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.TimeLord;
            else Color = Patches.Colors.Crew;
            StartRewind = DateTime.UtcNow.AddSeconds(-10.0f);
            FinishRewind = DateTime.UtcNow;
            RoleType = RoleEnum.TimeLord;
            Faction = Faction.Crewmates;
            Scale = 1.4f;
            UsesLeft = CustomGameOptions.RewindMaxUses;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewSupport;
            AlignmentName = "Crew (Support)";
            AddToRoleHistory(RoleType);
        }

        public DateTime StartRewind { get; set; }
        public DateTime FinishRewind { get; set; }

        public float TimeLordRewindTimer()
        {
            var utcNow = DateTime.UtcNow;


            TimeSpan timespan;
            float num;

            if (RecordRewind.rewinding)
            {
                timespan = utcNow - StartRewind;
                num = CustomGameOptions.RewindDuration * 1000f / 3f;
            }
            else
            {
                timespan = utcNow - FinishRewind;
                num = CustomGameOptions.RewindCooldown * 1000f;
            }


            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float GetCooldown()
        {
            return RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomGameOptions.RewindCooldown;
        }
    }
}