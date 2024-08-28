namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Survivor : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float VestCd { get; set; } = 25f;

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static float VestDur { get; set; } = 10f;

    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static int MaxVests { get; set; } = 5;

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
        BaseStart();
        Alignment = Alignment.NeutralBen;
        Objectives = () => "- Live to the end of the game";
        VestButton = CreateButton(this, new SpriteName("Vest"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)HitVest, new Cooldown(VestCd), "VEST", new Duration(VestDur),
            MaxVests);
    }

    public void HitVest()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, VestButton);
        VestButton.Begin();
    }
}