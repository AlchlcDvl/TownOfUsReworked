namespace TownOfUsReworked.PlayerLayers.Objectifiers;

public abstract class Objectifier : PlayerLayer
{
    public static List<Objectifier> AllObjectifiers => [ .. AllLayers.Where(x => x.LayerType == PlayerLayerEnum.Objectifier).Cast<Objectifier>() ];
    // public static readonly Dictionary<byte, Objectifier> ObjectifierLookup = [];
    public static Objectifier LocalObjectifier => CustomPlayer.Local.GetObjectifier();

    public override UColor Color => CustomColorManager.Objectifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Objectifier;
    public override LayerEnum Type => LayerEnum.NoneObjectifier;

    public virtual string Symbol => "φ";

    public static bool ObjectifierWins => WinState is WinLose.LoveWins or WinLose.RivalWins or WinLose.OverlordWins or WinLose.CorruptedWins or WinLose.DefectorWins or WinLose.MafiaWins or
        WinLose.TaskmasterWins;

    public string ColoredSymbol => $"{ColorString}{Symbol}</color>";

    public override void Init()
    {
        base.Init();
        Player.GetRole().LinkedObjectifier = Type;
    }
}