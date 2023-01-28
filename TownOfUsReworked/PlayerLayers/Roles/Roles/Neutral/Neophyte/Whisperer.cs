using System;
using System.Collections.Generic;
using Reactor.Utilities.Extensions;
using System.Reflection;
using TMPro;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.Extensions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Whisperer : Role
    {

        private KillButton _whisperButton;
        public DateTime LastWhispered;
        public int WhisperCount;
        public int ConversionCount;
        public List<(PlayerControl, int)> PlayerConversion = new List<(PlayerControl, int)>();
        public float WhisperConversion = CustomGameOptions.InitialWhisperRate;


        public Whisperer(PlayerControl player) : base(player)
        {
            Name = "Whisperer";
            Color = Colors.Whisperer;
            LastWhispered = DateTime.UtcNow;
            RoleType = RoleEnum.Whisperer;
        }

        public KillButton WhisperButton
        {
            get => _whisperButton;
            set
            {
                _whisperButton = value;
                AddToExtraButtons(value);
            }
        }

        public override void Wins()
        {
            SectWin = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(SubFaction) && player != PlayerControl.LocalPlayer)
                    team.Add(player);
            }

            __instance.teamToShow = team;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

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
            
            return false;
        }

        public float WhisperTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastWhispered;
            var num = (CustomGameOptions.WhisperCooldown + (CustomGameOptions.WhisperCooldownIncrease * WhisperCount)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public List<(PlayerControl, int)> GetPlayers()
        {
            var playerList = new List<(PlayerControl, int)>();

            foreach (var player in PlayerControl.AllPlayerControls)
                playerList.Add((player, 100));

            return playerList;
        }
    }
}