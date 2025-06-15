namespace TownOfUsReworked.PlayerLayers.Dispositions;

public abstract class Disposition : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Disposition;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Disposition;
    public override Layer Type => Layer.NoneDisposition;
    protected override UColor LayerColor => CustomColorManager.Disposition;
    protected override bool UseMainColor => ClientOptions.CustomDispColors;

    public virtual string Symbol => "φ";

    public string ColoredSymbol => $"{ColorString}{Symbol}</color>";
}