using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;

namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Overlord : Objectifier
    {
        public int OverlordMeetingCount = 0;
        public bool IsAlive => !(Player.Data.IsDead || Player.Data.Disconnected);
        public bool MeetingCountAchieved => OverlordMeetingCount >= CustomGameOptions.OverlordMeetingWinCount && IsAlive;

        public Overlord(PlayerControl player) : base(player)
        {
            Name = "Overlord";
            SymbolName = "β";
            TaskText = $"- Stay alive for {CustomGameOptions.OverlordMeetingWinCount} meetings.";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Overlord : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Overlord;
            Hidden = !CustomGameOptions.OverlordKnows;
        }
    }
}