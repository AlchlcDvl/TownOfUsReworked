using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using System;
using TownOfUsReworked.Classes;
using Hazel;
using TownOfUsReworked.PlayerLayers.Roles;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
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
            TaskText = "- Kill everyone!";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Corrupted : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Corrupted;
            ObjectifierDescription = "You are Corrupted! You are no longer Crew and feel the need to kill everyone!";
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                var role = Role.GetRole(Player);
                role.AddToAbilityButtons(value, role);
            }
        }

        public override void Wins() => CorruptedWin = true;

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (Utils.CorruptedWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
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