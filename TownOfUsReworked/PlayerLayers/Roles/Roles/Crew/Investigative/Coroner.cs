using System.Collections.Generic;
using System.Linq;
using GameObject = UnityEngine.Object;
using TownOfUsReworked.Modules;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Objects;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Coroner : CrewRole
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new();
        public List<byte> Reported = new();
        public AbilityButton CompareButton;
        public DeadBody CurrentTarget;
        public DeadPlayer ReferenceBody;
        public PlayerControl ClosestPlayer;
        public DateTime LastCompared;
        public DateTime LastAutopsied;
        public int UsesLeft;
        public bool ButtonUsable => UsesLeft > 0;
        public AbilityButton AutopsyButton;

        public Coroner(PlayerControl player) : base(player)
        {
            Name = "Coroner";
            StartText = "Examine The Dead For Info";
            AbilitiesText = "- You know when players die and will be notified to as to where their body is for a brief period of time.\n- When reporting a body, you get all of " +
                "the required info from it.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Coroner : Colors.Crew;
            Type = RoleEnum.Coroner;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            BodyArrows = new();
            Reported = new();
            InspectorResults = InspectorResults.DealsWithDead;
            UsesLeft = 0;
        }

        public float CompareTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastCompared;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.CompareCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float AutopsyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastAutopsied;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.AutopsyCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                GameObject.Destroy(arrow.Value);

            if (arrow.Value.gameObject != null)
                GameObject.Destroy(arrow.Value.gameObject);

            BodyArrows.Remove(arrow.Key);
        }

        public override void OnLobby()
        {
            BodyArrows.Values.DestroyAll();
            BodyArrows.Clear();
        }
    }
}