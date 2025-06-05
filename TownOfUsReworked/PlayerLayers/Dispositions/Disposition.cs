namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Disposition : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Disposition;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Disposition;
    public override LayerEnum Type => LayerEnum.NoneDisposition;
    protected override UColor LayerColor => CustomColorManager.Disposition;
    protected override bool UseMainColor => ClientOptions.CustomDispColors;

    public virtual string Symbol => "φ";

    public string ColoredSymbol => $"{ColorString}{Symbol}</color>";

    public static bool DispositionWins => WinState is WinLose.LoveWins or WinLose.RivalWins or WinLose.OverlordWins or WinLose.CorruptedWins or WinLose.DefectorWins or WinLose.MafiaWins or
        WinLose.TaskmasterWins;
}