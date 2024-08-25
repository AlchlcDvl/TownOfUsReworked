namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Professional : Modifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ProfessionalKnows { get; set; } = true;

    public bool LifeUsed { get; set; }

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Professional : CustomColorManager.Modifier;
    public override string Name => "Professional";
    public override LayerEnum Type => LayerEnum.Professional;
    public override Func<string> Description => () => "- You have an extra life when assassinating";
    public override bool Hidden => !ProfessionalKnows && !LifeUsed && !Dead;

    public override void Init() => LifeUsed = false;
}