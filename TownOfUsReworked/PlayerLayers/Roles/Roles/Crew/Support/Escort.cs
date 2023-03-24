using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Escort : CrewRole
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl BlockTarget;
        public bool Enabled;
        public DateTime LastBlock;
        public float TimeRemaining;
        public AbilityButton BlockButton;
        public bool Blocking => TimeRemaining > 0f;

        public Escort(PlayerControl player) : base(player)
        {
            Name = "Escort";
            RoleType = RoleEnum.Escort;
            StartText = "Roleblock Players And Stop Them From Harming Others";
            AbilitiesText = "- You can seduce players.\n- Seduction blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you attempt to block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Escort : Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            RoleBlockImmune = true;
            InspectorResults = InspectorResults.MeddlesWithOthers;
        }

        public void UnBlock()
        {
            Enabled = false;
            var targetRole = GetRole(BlockTarget);
            targetRole.IsBlocked = false;
            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || !BlockTarget.IsBlocked())
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBlock;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.EscRoleblockCooldown) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }
    }
}