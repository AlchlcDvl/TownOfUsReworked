namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Professional)]
public sealed class Professional : Modifier
{
    [ToggleOption]
    private static bool ProfessionalKnows = true;

    public bool LifeUsed { get; set; }

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Professional : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Professional;
    public override Func<string> Description => () => "- You have an extra life when assassinating";
    public override bool Hidden => !ProfessionalKnows && !LifeUsed && !Dead;
}