namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    public class Overlord : Objectifier
    {
        public bool IsAlive => !(IsDead || Disconnected);

        public Overlord(PlayerControl player) : base(player)
        {
            Name = "Overlord";
            Symbol = "β";
            TaskText = () => $"- Stay alive for {CustomGameOptions.OverlordMeetingWinCount} meetings";
            Color = CustomGameOptions.CustomObjectifierColors ? Colors.Overlord : Colors.Objectifier;
            ObjectifierType = ObjectifierEnum.Overlord;
            Hidden = !CustomGameOptions.OverlordKnows && !IsDead;
            Type = LayerEnum.Overlord;

            if (TownOfUsReworked.IsTest)
                Utils.LogSomething($"{Player.name} is {Name}");
        }
    }
}