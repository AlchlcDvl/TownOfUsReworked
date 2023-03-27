using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Data;
using TownOfUsReworked.Modules;
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
            InspectorResults = InspectorResults.TracksOthers;
        }

        public float TrackerTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastTracked;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.TrackCd) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
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

        protected internal override void OnLobby()
        {
            TrackerArrows.Values.DestroyAll();
            TrackerArrows.Clear();
        }
    }
}