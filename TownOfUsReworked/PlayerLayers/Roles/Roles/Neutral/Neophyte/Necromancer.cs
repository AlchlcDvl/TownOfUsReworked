using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using System;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Necromancer : NeutralRole
    {
        public bool CurrentlyReviving;
        public DeadBody CurrentTarget;
        public PlayerControl ClosestPlayer;
        public AbilityButton ResurrectButton;
        public AbilityButton KillButton;
        public List<byte> Resurrected = new();
        public int ResurrectUsesLeft;
        public bool ResurrectButtonUsable => ResurrectUsesLeft != 0;
        public int KillUsesLeft;
        public bool KillButtonUsable => KillUsesLeft != 0;
        public DateTime LastKilled;
        public DateTime LastResurrected;
        public int ResurrectedCount;
        public int KillCount;
        public bool Resurrecting;
        public float TimeRemaining;
        public bool IsResurrecting => TimeRemaining > 0f;

        public Necromancer(PlayerControl player) : base(player)
        {
            Name = "Necromancer";
            StartText = "Resurrect The Dead Into Doing Your Bidding";
            AbilitiesText = "- You can revive a dead body and bring them to your team.";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Necromancer : Colors.Neutral;
            RoleType = RoleEnum.Necromancer;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = NN;
            Objectives = "- Resurrect the dead into helping you gain control of the crew.";
            SubFaction = SubFaction.Reanimated;
            SubFactionColor = Colors.Reanimated;
            ResurrectUsesLeft = CustomGameOptions.ResurrectCount;
            KillUsesLeft = CustomGameOptions.NecroKillCount;
            ResurrectedCount = 0;
            KillCount = 0;
            Resurrected = new List<byte>
            {
                Player.PlayerId
            };
        }

        public float ResurrectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastResurrected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ResurrectCooldown, ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.NecroKillCooldown, KillCount * CustomGameOptions.NecroKillCooldownIncrease) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public void Resurrect()
        {
            Resurrecting = true;
            TimeRemaining -= Time.deltaTime;

            if (MeetingHud.Instance || Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void UnResurrect()
        {
            Resurrecting = false;
            LastResurrected = DateTime.UtcNow;
        }
    }
}