namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Modifier : PlayerLayer
{
    public static Modifier LocalModifier => CustomPlayer.Local.GetModifier();

    public override UColor Color => CustomColorManager.Modifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;
    public override LayerEnum Type => LayerEnum.NoneModifier;

    public static IEnumerable<Modifier> AllModifiers() => AllLayers.Where(x => x.LayerType == PlayerLayerEnum.Modifier).Cast<Modifier>();
}