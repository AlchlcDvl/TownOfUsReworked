namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Veteran)]
public sealed class Veteran : CKilling, IAlerter
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxAlerts = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number AlertCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number AlertDur = 10;

    public CustomButton AlertButton { get; private set; }

    protected override UColor MainColor => CustomColorManager.Veteran;
    public override Layer Type => Layer.Veteran;
    public override string StartText => "Alert To Kill Anyone Who Dares To Touch You";
    public override string Description => "- You can go on alert\n- When on alert, you will kill whoever interacts with you";
    public override Defense Defense => AlertButton.EffectActive ? Defense.Basic : Defense.None;
    public override Attack Attack => AlertButton.EffectActive ? Attack.Powerful : Attack.None;

    public override void Init() => AlertButton ??= new(this, "ALERT", new SpriteName("Alert"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Alert, new Cooldown(AlertCd),
        MaxAlerts, (EndFunc)EndEffect, new Duration(AlertDur));

    private void Alert()
    {
        Play("Alert");
        AlertButton.TriggerRpcAndBegin();
    }

    private bool EndEffect() => Dead;
}