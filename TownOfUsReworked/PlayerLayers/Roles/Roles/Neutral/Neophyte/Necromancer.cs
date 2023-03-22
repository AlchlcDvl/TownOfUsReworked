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
        public bool CurrentlyReviving = false;
        public DeadBody CurrentTarget = null;
        public PlayerControl ClosestPlayer;
        public AbilityButton ResurrectButton;
        public AbilityButton KillButton;
        public List<byte> Resurrected;
        public int ResurrectUsesLeft;
        public bool ResurrectButtonUsable => ResurrectUsesLeft != 0;
        public int KillUsesLeft;
        public bool KillButtonUsable => KillUsesLeft != 0;
        public DateTime LastKilled;
        public DateTime LastResurrected;
        public int ResurrectedCount;
        public int KillCount;
        public bool Resurrecting = false;
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
            Resurrected = new List<byte>();
            Resurrected.Add(Player.PlayerId);
            ResurrectUsesLeft = CustomGameOptions.ResurrectCount;
            KillUsesLeft = CustomGameOptions.NecroKillCount;
            ResurrectedCount = 0;
            KillCount = 0;
        }

        public float ResurrectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastResurrected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ResurrectCooldown, ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.NecroKillCooldown, KillCount * CustomGameOptions.NecroKillCooldownIncrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0f;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
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