namespace TownOfUsReworked.PlayerLayers.Roles;

// FIXME: Doesn't actually go invisible
[LayerHeaderOption(LayerEnum.Chameleon)]
public sealed class Chameleon : Crew
{
    [NumberOption(0, 15, 1, zeroIsInf: true)]
    public static Number MaxSwoops = 5;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SwoopCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number SwoopDur = 10;

    private CustomButton SwoopButton { get; set; }
    private bool ClickedAgain { get; set; }

    protected override UColor MainColor => CustomColorManager.Chameleon;
    public override LayerEnum Type => LayerEnum.Chameleon;
    public override Func<string> StartText { get; } = () => "Go Invisible To Stalk Players";
    public override Func<string> Description => () => "- You can turn invisible";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Support;
        SwoopButton ??= new(this, "SWOOP", new SpriteName("Swoop"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Swoop, new Cooldown(SwoopCd), (EffectStartVoid)Invis,
            MaxSwoops, new Duration(SwoopDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect, (ClickedAgainVoid)ClickAgain);
    }

    private void Invis() => MiscUtils.Invis(Player, SwoopDur, EndEffect);

    private void UnInvis() => ClickedAgain = false;

    private void ClickAgain() => ClickedAgain = true;

    private void Swoop()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, SwoopButton);
        SwoopButton.Begin();
    }

    private bool EndEffect() => Dead || ClickedAgain;
}