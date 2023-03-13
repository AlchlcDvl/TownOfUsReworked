using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Tracker : CrewRole
    {
        public Dictionary<byte, ArrowBehaviour> TrackerArrows;
        public PlayerControl ClosestPlayer;
        public DateTime LastTracked;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public AbilityButton TrackButton;

        public Tracker(PlayerControl player) : base(player)
        {
            Name = "Tracker";
            StartText = "Stalk Everyone To Monitor Their Movements";
            AbilitiesText = $"- You can track players which creates arrows that update every now and then.\n- You have {UsesLeft} bugs left.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Tracker : Colors.Crew;
            RoleType = RoleEnum.Tracker;
            UsesLeft = CustomGameOptions.MaxTracks;
            TrackerArrows = new Dictionary<byte, ArrowBehaviour>();
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            RoleDescription = "You are a Tracker! You can place bugs on players that update you on their locations. Find out who's lying about where they are!";
            InspectorResults = InspectorResults.TracksOthers;
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastTracked;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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