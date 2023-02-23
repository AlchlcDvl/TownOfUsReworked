using TownOfUsReworked.Enums;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.CustomOptions;
using System;
using TownOfUsReworked.Classes;
using Hazel;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Escort : Role
    {
        public PlayerControl ClosestPlayer;
        public PlayerControl BlockTarget;
        public bool Enabled = false;
        public DateTime LastBlock { get; set; }
        public float TimeRemaining;
        private KillButton _blockButton;
        public bool Blocking => TimeRemaining > 0f;

        public Escort(PlayerControl player) : base(player)
        {
            Name = "Escort";
            Faction = Faction.Crew;
            RoleType = RoleEnum.Escort;
            StartText = "Roleblock Players And Stop Them From Harming Others";
            AbilitiesText = "- You can seduce players.\n- Seduction blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you attempt to block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Color = CustomGameOptions.CustomCrewColors ? Colors.Escort : Colors.Crew;
            FactionName = "Crew";
            FactionColor = Colors.Crew;
            RoleAlignment = RoleAlignment.CrewSupport;
            AlignmentName = CS;
            Objectives = CrewWinCon;
            RoleDescription = "You are an Escort! You can have a little bit of \"fun time\" with players to ensure they are unable to kill anyone.";
            RoleBlockImmune = true;
            InspectorResults = InspectorResults.MeddlesWithOthers;
        }

        public KillButton BlockButton
        {
            get => _blockButton;
            set
            {
                _blockButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.EvilRecruit);
            }

            __instance.teamToShow = team;
        }

		public void UnBlock()
		{
			Enabled = false;
			BlockTarget = null;
            LastBlock = DateTime.UtcNow;
			Utils.DefaultOutfit(Player);
		}

		public void Block()
		{
			Enabled = true;
			TimeRemaining -= Time.deltaTime;

            var targetRole = GetRole(BlockTarget);
            targetRole.IsBlocked = !targetRole.RoleBlockImmune;

			if (Player.Data.IsDead)
				TimeRemaining = 0f;
		}

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlock;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.EscRoleblockCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsIntTraitor || IsIntFanatic)
                IntruderWin = true;
            else if (IsSynTraitor || IsSynFanatic)
                SyndicateWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else
                CrewWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && Utils.CabalWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsIntTraitor || IsIntFanatic) && Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if ((IsSynTraitor || IsSynFanatic) && Utils.SyndicateWins())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && Utils.SectWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && Utils.UndeadWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && Utils.ReanimatedWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.CrewWins() && NotDefective)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }
    }
}