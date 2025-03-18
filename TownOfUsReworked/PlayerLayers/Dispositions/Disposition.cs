namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Disposition : PlayerLayer
{
    public override UColor MainColor => CustomColorManager.Disposition;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Disposition;
    public override LayerEnum Type => LayerEnum.NoneDisposition;
    public override UColor LayerColor => CustomColorManager.Disposition;
    public override bool UseMainColor => ClientOptions.CustomDispColors;

    public virtual string Symbol => "φ";

    public string ColoredSymbol => $"{ColorString}{Symbol}</color>";

    public static bool DispositionWins => WinState is WinLose.LoveWins or WinLose.RivalWins or WinLose.OverlordWins or WinLose.CorruptedWins or WinLose.DefectorWins or WinLose.MafiaWins or
        WinLose.TaskmasterWins;

    protected override void Init() => Player.GetRole().LinkedDisposition = Type;
}