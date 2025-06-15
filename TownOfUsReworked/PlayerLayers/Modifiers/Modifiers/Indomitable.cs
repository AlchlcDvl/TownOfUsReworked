namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(Layer.Indomitable)]
public sealed class Indomitable : Modifier
{
    [ToggleOption]
    private static bool IndomitableKnows = true;

    public bool AttemptedGuess { get; set; }

    protected override UColor MainColor => CustomColorManager.Indomitable;
    public override Layer Type => Layer.Indomitable;
    public override string Description => "- You cannot be guessed";
    public override bool Hidden => !IndomitableKnows && !AttemptedGuess && !Dead;
}