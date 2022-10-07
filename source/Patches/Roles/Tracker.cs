using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TMPro;

namespace TownOfUs.Roles
{
    public class Tracker : Role
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
        public PlayerControl ClosestPlayer;
        public DateTime LastTracked { get; set; }

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            ImpostorText = () => "Stalk Everyone To Monitor Their Movements";
            TaskText = () => "Track suspicious players";
            if (CustomGameOptions.CustomCrewColors) Color = Patches.Colors.Tracker;
            else Color = Patches.Colors.Crew;
            LastTracked = DateTime.UtcNow;
            RoleType = RoleEnum.Tracker;
            Faction = Faction.Crewmates;
            UsesLeft = CustomGameOptions.MaxTracks;
            FactionName = "Crew";
            FactionColor = Patches.Colors.Crew;
            Alignment = Alignment.CrewInvest;
            AlignmentName = "Crew (Investigative)";
            AddToRoleHistory(RoleType);
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = CustomGameOptions.TrackCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public bool IsTracking(PlayerControl player)
        {
            return TrackerArrows.ContainsKey(player.PlayerId);
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = TrackerArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);
            TrackerArrows.Remove(arrow.Key);
        }
    }
}