namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Survivor)]
public sealed class Survivor : Neutral
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number VestCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number VestDur = 10;

    [NumberOption(0, 15, 1, zeroIsInf: true)]
    private static Number MaxVests = 5;

    [ToggleOption]
    public static bool SurvVent = false;

    [ToggleOption]
    public static bool SurvSwitchVent = false;

    public CustomButton VestButton { get; private set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Survivor : FactionColor;
    public override LayerEnum Type => LayerEnum.Survivor;
    public override Func<string> StartText => () => "Do Whatever It Takes To Live";
    public override Func<string> Description => () => "- You can put on a vest, which makes you unkillable for a short duration of time";
    public override DefenseEnum DefenseVal => VestButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Benign;
        Objectives = () => "- Live to the end of the game";
        VestButton ??= new(this, new SpriteName("Vest"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)HitVest, new Cooldown(VestCd), "VEST", new Duration(VestDur),
            MaxVests);
    }

    private void HitVest()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, VestButton);
        VestButton.Begin();
    }
}