using TownOfUsReworked.Patches;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using Hazel;

namespace TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers
{
    public class Overlord : Objectifier
    {
        public int OverlordMeetingCount = 0;
        public bool IsAlive => !(Player.Data.IsDead || Player.Data.Disconnected);
        public bool OverlordWins { get; set; }

        public Overlord(PlayerControl player) : base(player)
        {
            Name = "Overlord";
            SymbolName = "β";
            TaskText = "Take over the mission!";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Overlord : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Overlord;
        }

        internal override bool GameEnd(ShipStatus __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;
                
            if (MeetingCountAchieved())
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.OverlordWin, SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public override void Wins()
        {
            OverlordWins = true;
        }

        private bool MeetingCountAchieved()
        {
            var flag = false;

            if (OverlordMeetingCount >= CustomGameOptions.OverlordMeetingWinCount && IsAlive)
                flag = true;

            return flag;
        }
    }
}