namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Modifier : PlayerLayer
{
    public static List<Modifier> AllModifiers => [ .. AllLayers.Where(x => x.LayerType == PlayerLayerEnum.Modifier).Cast<Modifier>() ];
    // public static readonly Dictionary<byte, Modifier> ModifierLookup = [];
    public static Modifier LocalModifier => CustomPlayer.Local.GetModifier();

    public override UColor Color => CustomColorManager.Modifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;
    public override LayerEnum Type => LayerEnum.NoneModifier;
}