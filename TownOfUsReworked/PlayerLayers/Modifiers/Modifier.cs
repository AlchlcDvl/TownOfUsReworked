namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Modifier : PlayerLayer
{
    public static readonly List<Modifier> AllModifiers = new();
    public static Modifier LocalModifier => CustomPlayer.Local.GetModifier();

    public override UColor Color => CustomColorManager.Modifier;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Modifier;
    public override LayerEnum Type => LayerEnum.NoneModifier;

    protected Modifier() : base() => AllModifiers.Add(this);
}