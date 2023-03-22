using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Classes;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Consort : IntruderRole
    {
        public DateTime LastBlock;
        public float TimeRemaining;
        public AbilityButton BlockButton;
        public PlayerControl BlockTarget;
        public bool Enabled = false;
        public bool Blocking => TimeRemaining > 0f;
        public PlayerControl ClosestTarget;

        public Consort(PlayerControl player) : base(player)
        {
            Name = "Consort";
            RoleType = RoleEnum.Consort;
            StartText = "Roleblock The Crew And Stop Them From Progressing";
            AbilitiesText = "- You can seduce players.\n- Seduction blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consort : Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = IS;
            RoleBlockImmune = true;
        }

        public void UnBlock()
        {
            Enabled = false;

            var targetRole = GetRole(BlockTarget);
            targetRole.IsBlocked = false;

            BlockTarget = null;
            LastBlock = DateTime.UtcNow;
            Utils.DefaultOutfit(Player);
        }

        public void Block()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;

            if (Player.Data.IsDead || BlockTarget.Data.IsDead || BlockTarget.Data.Disconnected || MeetingHud.Instance)
                TimeRemaining = 0f;
        }

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlock;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ConsRoleblockCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}