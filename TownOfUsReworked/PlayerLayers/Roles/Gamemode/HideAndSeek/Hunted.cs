namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Hunted)]
public sealed class Hunted : HideAndSeek
{
    [NumberOption(1f, 2f, 0.05f, Format.Multiplier)]
    public static Number HuntedVision = 1.5f;

    [ToggleOption]
    public static bool HuntedFlashlight = false;

    [ToggleOption]
    public static bool HuntedChat = true;

    public override LayerEnum Type => LayerEnum.Hunted;
    public override Func<string> StartText { get; } = () => "Run, Hide And Do Tasks";
    protected override UColor MainColor => CustomColorManager.Hunted;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without being hunted";
    }
}