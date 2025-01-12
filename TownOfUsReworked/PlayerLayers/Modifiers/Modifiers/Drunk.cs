namespace TownOfUsReworked.PlayerLayers.Modifiers;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Drunk : Modifier
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool DrunkControlsSwap { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool DrunkKnows { get; set; } = true;

    [NumberOption(MultiMenu.LayerSubOptions, 1f, 20f, 1f, Format.Time)]
    public static Number DrunkInterval { get; set; } = new(10);

    private static float _time;
    public int Modify { get; set; }
    private bool Exposed { get; set; }

    public override UColor Color => ClientOptions.CustomModColors ? CustomColorManager.Drunk : CustomColorManager.Modifier;
    public override LayerEnum Type => LayerEnum.Drunk;
    public override Func<string> Description => () => DrunkControlsSwap ? "- Your controls swap over time" : "- Your controls are inverted";
    public override bool Hidden => !DrunkKnows && !Exposed && !Dead;

    public override void Init()
    {
        Modify = Hidden ? 1 : -1;
        Exposed = DrunkKnows;
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (DrunkControlsSwap)
        {
            _time += Time.deltaTime;

            if (_time >= DrunkInterval)
            {
                _time -= DrunkInterval;
                Modify *= -1;
                Exposed = true;
            }
        }
    }
}