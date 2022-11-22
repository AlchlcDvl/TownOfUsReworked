using Hazel;
using System;
using System.Linq;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Pestilence : Role
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool PestilenceWins { get; set; }

        public Pestilence(PlayerControl owner) : base(owner)
        {
            Name = "Pestilence";
            ImpostorText = () => "The Horseman Of The Apocalypse Has Arrived!";
            TaskText = () => "Kill everyone with your unstoppable abilities!\nFake Tasks:";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Pestilence : Colors.Neutral;
            SubFaction = SubFaction.None;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Pestilence;
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = () => "Neutral (Killing)";
            IntroText = "Obliterate those who oppose you";
            Results = InspResults.GFMayorRebelPest;
            AddToRoleHistory(RoleType);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                (x.Data.IsImpostor() | (x.Is(RoleAlignment.NeutralKill) && !x.Is(RoleEnum.Pestilence) && !x.Is(RoleEnum.Plaguebearer)) |
                x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros) | x.Is(Faction.Crew))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.PestilenceWin,
                    SendOption.Reliable, -1);
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

            if (flag2)
                return 0;

            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}