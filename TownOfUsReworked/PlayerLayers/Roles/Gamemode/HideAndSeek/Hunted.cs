namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Hunted : HideAndSeek
{
    public override LayerEnum Type => LayerEnum.Hunted;
    public override Func<string> StartText => () => "Run, Hide And Do Tasks";
    protected override UColor MainColor => CustomColorManager.Hunted;
    public override float VisionRange => GameModeSettings.HuntedVision;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without being hunted";
    }
}