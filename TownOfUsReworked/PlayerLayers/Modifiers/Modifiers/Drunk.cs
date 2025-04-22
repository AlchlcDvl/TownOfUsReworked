namespace TownOfUsReworked.PlayerLayers.Modifiers;

[LayerHeaderOption(LayerEnum.Drunk)]
public sealed class Drunk : Modifier
{
    [ToggleOption]
    private static bool DrunkControlsSwap = false;

    [ToggleOption]
    private static bool DrunkKnows = true;

    [NumberOption(1f, 20f, 1f, Format.Time)]
    private static Number DrunkInterval = 10;

    private static float Time;
    public int Modify { get; private set; }
    private bool Exposed { get; set; }

    protected override UColor MainColor => CustomColorManager.Drunk;
    public override LayerEnum Type => LayerEnum.Drunk;
    public override Func<string> Description => () => DrunkControlsSwap ? "- Your controls swap over time" : "- Your controls are inverted";
    public override bool Hidden => !DrunkKnows && !Exposed && !Dead;

    protected override void Init()
    {
        Modify = Hidden ? 1 : -1;
        Exposed = DrunkKnows;
    }

    public override void UpdateHud(HudManager __instance)
    {
        if (!DrunkControlsSwap)
            return;

        Time += UnityEngine.Time.deltaTime;

        if (Time < DrunkInterval)
            return;

        Time -= DrunkInterval;
        Modify *= -1;
        Exposed = true;
    }
}