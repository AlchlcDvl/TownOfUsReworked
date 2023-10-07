namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public abstract class Objectifier : PlayerLayer
{
    public static readonly List<Objectifier> AllObjectifiers = new();
    public static Objectifier LocalObjectifier => GetObjectifier(CustomPlayer.Local);

    public override Color Color => Colors.Objectifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Objectifier;

    public virtual string Symbol => "φ";
    public virtual bool Hidden => false;

    public static bool LoveWins { get; set; }
    public static bool RivalWins { get; set; }
    public static bool TaskmasterWins { get; set; }
    public static bool CorruptedWins { get; set; }
    public static bool OverlordWins { get; set; }
    public static bool MafiaWins { get; set; }

    public static bool ObjectifierWins => LoveWins || RivalWins || TaskmasterWins || CorruptedWins || OverlordWins || MafiaWins;

    protected Objectifier(PlayerControl player) : base(player)
    {
        if (GetObjectifier(player))
            GetObjectifier(player).Player = null;

        AllObjectifiers.Add(this);
    }

    public string ColoredSymbol => $"{ColorString}{Symbol}</color>";

    public static Objectifier GetObjectifier(PlayerControl player) => AllObjectifiers.Find(x => x.Player == player);

    public static T GetObjectifier<T>(PlayerControl player) where T : Objectifier => GetObjectifier(player) as T;

    public static Objectifier GetObjectifier(PlayerVoteArea area) => GetObjectifier(PlayerByVoteArea(area));

    public static List<Objectifier> GetObjectifiers(LayerEnum objectifiertype) => AllObjectifiers.Where(x => x.Type == objectifiertype).ToList();

    public static List<T> GetObjectifiers<T>(LayerEnum objectifiertype) where T : Objectifier => GetObjectifiers(objectifiertype).Cast<T>().ToList();
}