using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class TimeLord : CrewRole
    {
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public DateTime StartRewind;
        public DateTime FinishRewind;
        public AbilityButton RewindButton;

        public TimeLord(PlayerControl player) : base(player)
        {
            Name = "Time Lord";
            StartText = "Rewind Time To Harass The <color=#FF0000FF>Evildoers</color>";
            AbilitiesText = $"- You can rewind time, which will force players to move back to a previous location.\n- You have {UsesLeft} rewinds left." + (CustomGameOptions.RewindRevive ?
                $"\n- Rewinding time will also revive anyone who has died in the last {CustomGameOptions.RewindDuration}s." : "");
            Color = CustomGameOptions.CustomCrewColors ? Colors.TimeLord : Colors.Crew;
            StartRewind = DateTime.UtcNow.AddSeconds(-10.0f);
            FinishRewind = DateTime.UtcNow;
            RoleType = RoleEnum.TimeLord;
            UsesLeft = CustomGameOptions.RewindMaxUses;
            RoleAlignment = RoleAlignment.CrewProt;
            AlignmentName = CP;
            InspectorResults = InspectorResults.DifferentLens;
            RoleBlockImmune = true;
        }

        public static float GetCooldown() => RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomGameOptions.RewindCooldown;

        public float TimeLordRewindTimer()
        {
            var utcNow = DateTime.UtcNow;
            var num = (RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomButtons.GetModifiedCooldown(CustomGameOptions.RewindCooldown)) * 1000f / (RecordRewind.rewinding ? 3f : 1f);
            var timespan = utcNow - (RecordRewind.rewinding ? StartRewind : FinishRewind);
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}