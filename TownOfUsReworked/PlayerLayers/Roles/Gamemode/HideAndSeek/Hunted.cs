namespace TownOfUsReworked.PlayerLayers.Roles;

public sealed class Hunted : HideAndSeek
{
    public override LayerEnum Type => LayerEnum.Hunted;
    public override Func<string> StartText { get; } = () => "Run, Hide And Do Tasks";
    protected override UColor MainColor => CustomColorManager.Hunted;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Finish your tasks without being hunted";
    }
}