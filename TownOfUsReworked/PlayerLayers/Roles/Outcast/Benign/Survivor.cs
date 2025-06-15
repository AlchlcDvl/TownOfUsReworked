namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Survivor)]
public sealed class Survivor : Benign
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number VestCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number VestDur = 10;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    private static Number MaxVests = 5;

    [ToggleOption]
    private static bool SurvVent = false;

    [ToggleOption]
    private static bool SurvSwitchVent = false;

    public CustomButton VestButton;

    protected override UColor MainColor => CustomColorManager.Survivor;
    public override Layer Type => Layer.Survivor;
    public override string StartText => "Do Whatever It Takes To Live";
    public override string Description => "- You can put on a vest, which gives you basic defense for a short duration of time";
    public override Defense Defense => VestButton.EffectActive ? Defense.Basic : Defense.None;
    public override bool CanVent => base.CanVent && SurvVent;
    public override bool CanSwitchVents => SurvSwitchVent;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Live to the end of the game";
        VestButton ??= new(this, new SpriteName("Vest"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitVest, new Cooldown(VestCd), "VEST", new Duration(VestDur),
            MaxVests);
    }

    private void HitVest()
    {
        CallRpc(ReworkedRpc.Action, ActionsRpc.ButtonAction, VestButton);
        VestButton.Begin();
    }
}