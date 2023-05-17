namespace TownOfUsReworked.PlayerLayers.Objectifiers
{
    [HarmonyPatch]
    public class Objectifier : PlayerLayer
    {
        public static readonly List<Objectifier> AllObjectifiers = new();
        public static Objectifier LocalObjectifier => GetObjectifier(PlayerControl.LocalPlayer);

        #pragma warning disable
        public static bool LoveWins;
        public static bool RivalWins;
        public static bool TaskmasterWins;
        public static bool CorruptedWins;
        public static bool OverlordWins;
        public static bool MafiaWins;
        #pragma warning restore

        public static bool ObjectifierWins => LoveWins || RivalWins || TaskmasterWins || CorruptedWins || OverlordWins || MafiaWins;

        public Objectifier(PlayerControl player) : base(player)
        {
            Color = Colors.Objectifier;
            LayerType = PlayerLayerEnum.Objectifier;
            AllObjectifiers.Add(this);
        }

        public string Symbol = "φ";
        public string TaskText = "- None";
        public bool Hidden;

        public string ColoredSymbol => $"{ColorString}{Symbol}</color>";

        public static Objectifier GetObjectifier(PlayerControl player) => AllObjectifiers.Find(x => x.Player == player);

        public static T GetObjectifier<T>(PlayerControl player) where T : Objectifier => GetObjectifier(player) as T;

        public static Objectifier GetObjectifier(PlayerVoteArea area) => GetObjectifier(Utils.PlayerByVoteArea(area));

        public static List<Objectifier> GetObjectifiers(ObjectifierEnum objectifiertype) => AllObjectifiers.Where(x => x.ObjectifierType == objectifiertype).ToList();

        public static List<T> GetObjectifiers<T>(ObjectifierEnum objectifiertype) where T : Objectifier => GetObjectifiers(objectifiertype).Cast<T>().ToList();
    }
}