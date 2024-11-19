namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Survivor : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number VestCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number VestDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxVests { get; set; } = new(5);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SurvVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SurvSwitchVent { get; set; } = false;

    public CustomButton VestButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Survivor : CustomColorManager.Neutral;
    public override string Name => "Survivor";
    public override LayerEnum Type => LayerEnum.Survivor;
    public override Func<string> StartText => () => "Do Whatever It Takes To Live";
    public override Func<string> Description => () => "- You can put on a vest, which makes you unkillable for a short duration of time";
    public override DefenseEnum DefenseVal => VestButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Live to the end of the game";
        VestButton ??= new(this, new SpriteName("Vest"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)HitVest, new Cooldown(VestCd), "VEST", new Duration(VestDur), MaxVests);
    }

    public void HitVest()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, VestButton);
        VestButton.Begin();
    }
}