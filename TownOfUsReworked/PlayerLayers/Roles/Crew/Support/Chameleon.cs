namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Chameleon)]
public sealed class Chameleon : CSupport
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxSwoops = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SwoopCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number SwoopDur = 10;

    private CustomButton SwoopButton;
    private bool ClickedAgain;

    protected override UColor MainColor => CustomColorManager.Chameleon;
    public override LayerEnum Type => LayerEnum.Chameleon;
    public override string StartText => "Go Invisible To Stalk Players";
    public override string Description => "- You can turn invisible";

    public override void Init()
    {
        base.Init();
        SwoopButton ??= new(this, "SWOOP", new SpriteName("Swoop"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Swoop, new Cooldown(SwoopCd), (EffectStartVoid)Invis,
            MaxSwoops, new Duration(SwoopDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect, (ClickedAgainVoid)ClickAgain);
    }

    private void Invis() => MiscUtils.Invis(Player, SwoopDur, EndEffect);

    private void UnInvis() => ClickedAgain = false;

    private void ClickAgain() => ClickedAgain = true;

    private void Swoop() => SwoopButton.TriggerRpcAndBegin();

    private bool EndEffect() => Dead || ClickedAgain;
}