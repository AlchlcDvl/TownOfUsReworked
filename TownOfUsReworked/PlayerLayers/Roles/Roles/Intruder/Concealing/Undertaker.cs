using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Undertaker : IntruderRole
    {
        public AbilityButton DragButton;
        public AbilityButton DropButton;
        public AbilityButton HideButton;
        public DateTime LastDragged;
        public DeadBody CurrentTarget;
        public DeadBody CurrentlyDragging;
        public Vent ClosestVent;

        public Undertaker(PlayerControl player) : base(player)
        {
            Name = "Undertaker";
            StartText = "Drag Bodies And Hide Them";
            AbilitiesText = "- You can drag bodies to prevent them from getting reported.\n- You can hide bodies in vents, which only select few roels can remove.";
            Color = CustomGameOptions.CustomIntColors? Colors.Undertaker : Colors.Intruder;
            LastDragged = DateTime.UtcNow;
            RoleType = RoleEnum.Undertaker;
            RoleAlignment = RoleAlignment.IntruderConceal;
            AlignmentName = IC;
            CurrentlyDragging = null;
            InspectorResults = InspectorResults.MeddlesWithDead;
        }

        public float DragTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastDragged;
            var num = CustomButtons.GetModifiedCooldown(CustomGameOptions.DragCd, CustomButtons.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}