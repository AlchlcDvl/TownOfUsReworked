namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Disposition : PlayerLayer
{
    public static List<Disposition> AllDispositions() => [ .. AllLayers.Where(x => x.LayerType == PlayerLayerEnum.Disposition).Cast<Disposition>() ];
    // public static readonly Dictionary<byte, Disposition> DispositionLookup = [];
    public static Disposition LocalDisposition => CustomPlayer.Local.GetDisposition();

    public override UColor Color => CustomColorManager.Disposition;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Disposition;
    public override LayerEnum Type => LayerEnum.NoneDisposition;

    public virtual string Symbol => "φ";

    public static bool DispositionWins => WinState is WinLose.LoveWins or WinLose.RivalWins or WinLose.OverlordWins or WinLose.CorruptedWins or WinLose.DefectorWins or WinLose.MafiaWins or
        WinLose.TaskmasterWins;

    public string ColoredSymbol => $"{ColorString}{Symbol}</color>";

    public override void Init()
    {
        base.Init();
        Player.GetRole().LinkedDisposition = Type;
    }
}