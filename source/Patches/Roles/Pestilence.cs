using Hazel;
using System;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Pestilence : Role
    {
        public Pestilence(PlayerControl owner) : base(owner)
        {
            Name = "Pestilence";
            ImpostorText = () => "The Horseman Of The Apocalypse Has Arrived!";
            TaskText = () => "Kill everyone with your unstoppable abilities!\nFake Tasks:";
            if (CustomGameOptions.CustomNeutColors) Color = Patches.Colors.Pestilence;
            else Color = Patches.Colors.Neutral;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Pestilence;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Patches.Colors.Neutral;
            Alignment = Alignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool PestilenceWins { get; set; }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 2 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || (x.Is(Alignment.NeutralKill) && !x.Is(RoleEnum.Pestilence) && !x.Is(RoleEnum.Plaguebearer)) || x.Is(Alignment.NeutralChaos) ||
                    x.Is(Alignment.NeutralPower) || x.Data.IsCrew())) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(
                    PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.PestilenceWin,
                    SendOption.Reliable,
                    -1
                );
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            PestilenceWins = true;
        }

        public void Loses()
        {
            LostByRPC = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.PestKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}