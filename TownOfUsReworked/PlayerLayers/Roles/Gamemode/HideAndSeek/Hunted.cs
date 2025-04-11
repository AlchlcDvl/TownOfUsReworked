namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Hunted : HideAndSeek
{
    public override LayerEnum Type { get; } = LayerEnum.Hunted;
    public override Func<string> StartText { get; } = () => "Run, Hide And Do Tasks";
    protected override UColor MainColor => CustomColorManager.Hunted;
    public override float VisionRange => GameModeSettings.HuntedVision;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without being hunted";
    }
}