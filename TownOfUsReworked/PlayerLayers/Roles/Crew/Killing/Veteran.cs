namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Veteran : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 0, 15, 1, ZeroIsInfinity = true)]
    public static Number MaxAlerts { get; set; } = new(5);

    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number AlertCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number AlertDur { get; set; } = new(10);

    public CustomButton AlertButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Veteran : CustomColorManager.Crew;
    public override string Name => "Veteran";
    public override LayerEnum Type => LayerEnum.Veteran;
    public override Func<string> StartText => () => "Alert To Kill Anyone Who Dares To Touch You";
    public override Func<string> Description => () => "- You can go on alert\n- When on alert, you will kill whoever interacts with you";
    public override DefenseEnum DefenseVal => AlertButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;
    public override AttackEnum AttackVal => AlertButton.EffectActive ? AttackEnum.Powerful : AttackEnum.None;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewKill;
        AlertButton ??= new(this, "ALERT", new SpriteName("Alert"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClick)Alert, new Cooldown(AlertCd), MaxAlerts, (EndFunc)EndEffect,
            new Duration(AlertDur));
    }

    public void Alert()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AlertButton);
        AlertButton.Begin();
    }

    public bool EndEffect() => Dead;
}