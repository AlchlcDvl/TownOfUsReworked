namespace TownOfUsReworked.PlayerLayers.Modifiers;

public sealed class Coward : Modifier
{
    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Coward : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Coward;
    public override Func<string> Description => () => "- You cannot report bodies";
}