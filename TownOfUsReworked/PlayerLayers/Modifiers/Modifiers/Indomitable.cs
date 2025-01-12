namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Indomitable : Modifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool IndomitableKnows { get; set; } = true;

    public bool AttemptedGuess { get; set; }

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Indomitable : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Indomitable;
    public override Func<string> Description => () => "- You cannot be guessed";
    public override bool Hidden => !IndomitableKnows && !AttemptedGuess && !Dead;
}