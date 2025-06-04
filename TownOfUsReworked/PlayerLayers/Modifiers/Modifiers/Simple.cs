namespace TownOfUsReworked.PlayerLayers.Modifiers;

public abstract class Simple(UColor color, LayerEnum type, Func<string> description) : Modifier
{
    protected override UColor MainColor => color;
    public override LayerEnum Type => type;
    public override Func<string> Description => description;

    protected Simple(UColor color, LayerEnum type, string description) : this(color, type, () => description) {}
}

public sealed class Coward() : Simple(CustomColorManager.Coward, LayerEnum.Coward, "- You cannot report bodies");

public sealed class Shy() : Simple(CustomColorManager.Shy, LayerEnum.Shy, "- You cannot call meetings");