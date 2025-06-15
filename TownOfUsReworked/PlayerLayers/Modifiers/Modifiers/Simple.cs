namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Simple(UColor color, Layer type, string description) : Modifier
{
    protected override UColor MainColor => color;
    public override Layer Type => type;
    public override string Description => description;
}

public sealed class Coward() : Simple(CustomColorManager.Coward, Layer.Coward, "- You cannot report bodies");

public sealed class Shy() : Simple(CustomColorManager.Shy, Layer.Shy, "- You cannot call meetings");