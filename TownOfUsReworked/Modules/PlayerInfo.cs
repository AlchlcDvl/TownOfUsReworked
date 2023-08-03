namespace TownOfUsReworked.Modules
{
    public class PlayerInfo
    {
        public readonly string PlayerName;
        public readonly string History;
        public readonly string CachedHistory;

        public PlayerInfo(string name, string history, string cache)
        {
            PlayerName = name;
            History = history;
            CachedHistory = cache;
        }
    }
}