using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using System;
using TownOfUsReworked.Extensions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Corrupted : Objectifier
    {
        private KillButton _killButton;
        public DateTime LastKilled { get; set; }
        public bool CorruptedWin { get; set; }
        public PlayerControl ClosestPlayer;

        public Corrupted(PlayerControl player) : base(player)
        {
            Name = "Corrupted";
            SymbolName = "δ";
            TaskText = "You are not <color=#8BFDFDFF>Crew</color> anymore!";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Corrupted : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Corrupted;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public override void Wins()
        {
            CorruptedWin = true;
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.CorruptedWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.CorruptedWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.CorruptedKillCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}