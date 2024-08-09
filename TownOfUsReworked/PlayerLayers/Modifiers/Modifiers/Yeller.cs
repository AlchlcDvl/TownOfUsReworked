namespace TownOfUsReworked.PlayerLayers.Modifiers;

public class Yeller : Modifier
{
    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Yeller : CustomColorManager.Modifier;
    public override string Name => "Yeller";
    public override LayerEnum Type => LayerEnum.Yeller;
    public override Func<string> Description => () => "- Everyone knows where you are";
}