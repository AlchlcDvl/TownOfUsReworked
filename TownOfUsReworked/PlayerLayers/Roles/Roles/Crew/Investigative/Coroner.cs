using System.Collections.Generic;
using System.Linq;
using GameObject = UnityEngine.Object;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Objects;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Coroner : CrewRole
    {
        public Dictionary<byte, ArrowBehaviour> BodyArrows;
        public List<byte> Reported;
        public AbilityButton CompareButton;
        public DeadBody CurrentTarget = null;
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
            RoleType = RoleEnum.Coroner;
            RoleAlignment = RoleAlignment.CrewInvest;
            AlignmentName = CI;
            BodyArrows = new Dictionary<byte, ArrowBehaviour>();
            Reported = new List<byte>();
            RoleDescription = "You are a Coroner! You are an expert in revealing information from dead bodies and are so skilled to the point you even know when someone dies!" +
                " Your strong skill makes you a very tempting target for evils so be careful when revealing information.";
            InspectorResults = InspectorResults.DealsWithDead;
            //IntroSound = "CoronerIntro";
            UsesLeft = 0;
        }

        public float CompareTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCompared;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.CompareCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float AutopsyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAutopsied;
            var num = 10000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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
    }
}