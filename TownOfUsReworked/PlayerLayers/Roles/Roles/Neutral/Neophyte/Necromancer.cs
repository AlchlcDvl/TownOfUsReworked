using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using System.Collections.Generic;
using TownOfUsReworked.Classes;
using Hazel;
using TMPro;
using System;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Necromancer : Role
    {
        public bool CurrentlyReviving = false;
        public DeadBody CurrentTarget = null;
        public PlayerControl ClosestPlayer;
        private KillButton _resurrectButton;
        private KillButton _killButton;
        public List<byte> Resurrected;
        public int ResurrectUsesLeft;
        public TextMeshPro ResurrectUsesText;
        public bool ResurrectButtonUsable => ResurrectUsesLeft != 0;
        public int KillUsesLeft;
        public TextMeshPro KillUsesText;
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
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralNeo;
            AlignmentName = NN;
            RoleDescription = "Your are a Necromancer! You can revive a dead person if you find their body. Be careful though, because it takes time" +
                " to revive someone and a meeting being called will kill both you and your target.";
            Objectives = "- Resurrect the dead into helping you gain control of the crew.";
            SubFaction = SubFaction.Reanimated;
            SubFactionName = "Reanimated";
            SubFactionColor = Colors.Reanimated;
            Resurrected = new List<byte>();
            Resurrected.Add(Player.PlayerId);
            ResurrectUsesLeft = CustomGameOptions.ResurrectCount;
            KillUsesLeft = CustomGameOptions.NecroKillCount;
            ResurrectedCount = 0;
            KillCount = 0;
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            team.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = team;
        }

        public float ResurrectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastResurrected;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.ResurrectCooldown, ResurrectedCount * CustomGameOptions.ResurrectCooldownIncrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.NecroKillCooldown, KillCount * CustomGameOptions.NecroKillCooldownIncrease) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton ResurrectButton
        {
            get => _resurrectButton;
            set
            {
                _resurrectButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public void Resurrect()
        {
            Resurrecting = true;
            TimeRemaining -= Time.deltaTime;
        }

        public void UnResurrect()
        {
            Resurrecting = false;
            LastResurrected = DateTime.UtcNow;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.ReanimatedWin())
            {
                ReanimatedWin = true;
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}