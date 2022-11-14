using System;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;
using TownOfUsReworked.Lobby.CustomOption;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class TimeLord : Role
    {
        public int UsesLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => UsesLeft != 0;
        public bool TLWin;
        public DateTime StartRewind { get; set; }
        public DateTime FinishRewind { get; set; }

        public TimeLord(PlayerControl player) : base(player)
        {
            Name = "Time Lord";
            ImpostorText = () => "Rewind Time";
            TaskText = () => "Rewind Time!";
            Color = CustomGameOptions.CustomCrewColors ? Colors.TimeLord : Colors.Crew;
            SubFaction = SubFaction.None;
            StartRewind = DateTime.UtcNow.AddSeconds(-10.0f);
            FinishRewind = DateTime.UtcNow;
            RoleType = RoleEnum.TimeLord;
            Faction = Faction.Crew;
            Scale = 1.4f;
            UsesLeft = CustomGameOptions.RewindMaxUses;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = () => "Crew (Support)";
            IntroText = "Eject all <color=#FF0000FF>evildoers</color>";
            Results = InspResults.TrackAltTLTM;
            AddToRoleHistory(RoleType);
        }

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

            if (flag2)
                return 0;

            return (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float GetCooldown()
        {
            return RecordRewind.rewinding ? CustomGameOptions.RewindDuration : CustomGameOptions.RewindCooldown;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__21 __instance)
        {
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }
    }
}