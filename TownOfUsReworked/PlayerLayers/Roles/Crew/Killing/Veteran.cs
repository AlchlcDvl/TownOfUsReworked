namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Veteran)]
public sealed class Veteran : Crew, IAlerter
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxAlerts = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number AlertCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number AlertDur = 10;

    public CustomButton AlertButton { get; private set; }

    public override UColor MainColor => CustomColorManager.Veteran;
    public override LayerEnum Type => LayerEnum.Veteran;
    public override Func<string> StartText => () => "Alert To Kill Anyone Who Dares To Touch You";
    public override Func<string> Description => () => "- You can go on alert\n- When on alert, you will kill whoever interacts with you";
    public override DefenseEnum DefenseVal => AlertButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;
    public override AttackEnum AttackVal => AlertButton.EffectActive ? AttackEnum.Powerful : AttackEnum.None;

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Killing;
        AlertButton ??= new(this, "ALERT", new SpriteName("Alert"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Alert, new Cooldown(AlertCd), MaxAlerts,
            (EndFunc)EndEffect, new Duration(AlertDur));
    }

    private void Alert()
    {
        Play("Alert");
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, AlertButton);
        AlertButton.Begin();
    }

    private bool EndEffect() => Dead;
}