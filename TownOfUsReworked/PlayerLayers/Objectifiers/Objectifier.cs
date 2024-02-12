namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public abstract class Objectifier : PlayerLayer
{
    public static readonly List<Objectifier> AllObjectifiers = new();
    public static Objectifier LocalObjectifier => CustomPlayer.Local.GetObjectifier();

    public override UColor Color => CustomColorManager.Objectifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Objectifier;
    public override LayerEnum Type => LayerEnum.NoneObjectifier;

    public virtual string Symbol => "φ";

    public static bool LoveWins { get; set; }
    public static bool RivalWins { get; set; }
    public static bool TaskmasterWins { get; set; }
    public static bool CorruptedWins { get; set; }
    public static bool OverlordWins { get; set; }
    public static bool MafiaWins { get; set; }
    public static bool DefectorWins { get; set; }

    public static bool ObjectifierWins => LoveWins || RivalWins || TaskmasterWins || CorruptedWins || OverlordWins || MafiaWins || DefectorWins;

    protected Objectifier() : base() => AllObjectifiers.Add(this);

    public string ColoredSymbol => $"{ColorString}{Symbol}</color>";
}