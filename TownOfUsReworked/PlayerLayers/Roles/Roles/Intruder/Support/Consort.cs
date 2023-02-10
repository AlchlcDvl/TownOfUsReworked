using TownOfUsReworked.Enums;
using Il2CppSystem.Collections.Generic;
using TownOfUsReworked.Lobby.CustomOption;
using System;
using TownOfUsReworked.Classes;
using Hazel;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Consort : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastBlock { get; set; }
        public DateTime LastKilled { get; set; }
        public float TimeRemaining;
        private KillButton _blockButton;
        private KillButton _killButton;
        public PlayerControl BlockTarget;
        public bool Enabled = false;
        public bool Blocking => TimeRemaining > 0f;

        public Consort(PlayerControl player) : base(player)
        {
            Name = "Consort";
            Faction = Faction.Intruder;
            RoleType = RoleEnum.Consort;
            StartText = "Roleblock The Crew And Stop Them From Progressing";
            AbilitiesText = "- You can seduce players.";
            AttributesText = "- Seduction blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Color = CustomGameOptions.CustomIntColors ? Colors.Consort : Colors.Intruder;
            FactionName = "Intruder";
            FactionColor = Colors.Intruder;
            RoleAlignment = RoleAlignment.IntruderSupport;
            AlignmentName = "Intruder (Support)";
            FactionDescription = IntruderFactionDescription;
            AlignmentDescription = ISDescription;
            Objectives = IntrudersWinCon;
            RoleDescription = "You are a Consort! You can have a little bit of \"fun time\" with players to ensure they are unable to stop you from killing" +
                " everyone.";
            RoleBlockImmune = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (!IsRecruit)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
            }
            else
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
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

        public float RoleblockTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlock;
            var num = CustomGameOptions.ConsRoleblockCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.IntKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void SetBlocked(PlayerControl blocked)
        {
            LastBlock = DateTime.UtcNow;
            BlockTarget = blocked;
            Coroutines.Start(Utils.Block(this, blocked));
        }

        public void RPCSetBlocked(PlayerControl blocked)
        {
            var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer3.Write((byte)ActionsRPC.ConsRoleblock);
            writer3.Write(blocked.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer3);
            SetBlocked(blocked);
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsPersuaded)
                SectWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else
                IntruderWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsPersuaded)
            {
                if (Utils.SectWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SectWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsResurrected)
            {
                if (Utils.ReanimatedWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ReanimatedWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void PerformBlock()
        {
        }

        public void PerformKill()
        {
            
        }
    }
}