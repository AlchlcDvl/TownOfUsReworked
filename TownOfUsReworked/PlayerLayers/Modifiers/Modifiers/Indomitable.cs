namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Indomitable)]
public sealed class Indomitable : Modifier
{
    [ToggleOption]
    private static bool IndomitableKnows = true;

    public bool AttemptedGuess { get; set; }

    protected override UColor MainColor => CustomColorManager.Indomitable;
    public override LayerEnum Type { get; } = LayerEnum.Indomitable;
    public override Func<string> Description => () => "- You cannot be guessed";
    public override bool Hidden => !IndomitableKnows && !AttemptedGuess && !Dead;
}