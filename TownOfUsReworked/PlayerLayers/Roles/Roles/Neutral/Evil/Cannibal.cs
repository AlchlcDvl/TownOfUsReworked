using System;
using System.Collections.Generic;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using System.Linq;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Cannibal : NeutralRole
    {
        public AbilityButton EatButton;
        public DeadBody CurrentTarget;
        public int EatNeed;
        private string Body => EatNeed == 1 ? "body" : "bodies";
        public bool CannibalWin;
        public DateTime LastEaten;
        public Dictionary<byte, ArrowBehaviour> BodyArrows = new();
        public bool EatWin => EatNeed <= 0;

        public Cannibal(PlayerControl player) : base(player)
        {
            Name = "Cannibal";
            StartText = "Eat The Bodies Of The Dead";
            RoleType = RoleEnum.Cannibal;
            AbilitiesText = "- You can consume a body, making it disappear from the game.\n- You know when someone dies, so you can find their body.";
            RoleAlignment = RoleAlignment.NeutralEvil;
            AlignmentName = NE;
            Color = CustomGameOptions.CustomNeutColors ? Colors.Cannibal : Colors.Neutral;
            Objectives = $"- Eat {EatNeed} {Body}.";
            BodyArrows = new();
            EatNeed = CustomGameOptions.CannibalBodyCount >= PlayerControl.AllPlayerControls._size / 2 ? PlayerControl.AllPlayerControls._size / 2 :
                CustomGameOptions.CannibalBodyCount; //Limits the max required bodies to 1/2 of lobby's size
        }

        public float EatTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastEaten;
            var num = CustomGameOptions.CannibalCd * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void DestroyArrow(byte targetPlayerId)
        {
            var arrow = BodyArrows.FirstOrDefault(x => x.Key == targetPlayerId);

            if (arrow.Value != null)
                Object.Destroy(arrow.Value);
            if (arrow.Value.gameObject != null)
                Object.Destroy(arrow.Value.gameObject);

            BodyArrows.Remove(arrow.Key);
        }
    }
}