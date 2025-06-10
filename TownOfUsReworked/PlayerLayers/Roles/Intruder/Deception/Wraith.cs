namespace TownOfUsReworked.PlayerLayers.Roles;

// FIXME: Doesn't actually go invisible
// FIXME: Somehow Wraith and Poisoner can't kill each other with a dead poisoned Monarch
[LayerHeaderOption(LayerEnum.Wraith)]
public sealed class Wraith : Intruder
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number InvisCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    public static Number InvisDur = 10;

    [ToggleOption]
    public static bool WraithVent = false;

    private CustomButton InvisButton { get; set; }
    private bool ClickedAgain { get; set; }

    protected override UColor MainColor => CustomColorManager.Wraith;
    public override LayerEnum Type => LayerEnum.Wraith;
    public override Func<string> StartText { get; } = () => "Sneaky Sneaky";
    public override Func<string> Description => () => $"- You can turn invisible\n{CommonAbilities}";
    public override bool CanVent => WraithVent;

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.Deception;
        InvisButton ??= new(this, "INVISIBILITY", new SpriteName("Invis"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitInvis, new Cooldown(InvisCd), (EffectStartVoid)Invis,
            new Duration(InvisDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect, (ClickedAgainVoid)ClickAgain);
    }

    private void Invis() => MiscUtils.Invis(Player, InvisDur, EndEffect, Player.IsBuddyWith(LocalPlayer, Faction));

    private void UnInvis() => ClickedAgain = false;

    private void ClickAgain() => ClickedAgain = true;

    private void HitInvis()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, InvisButton);
        InvisButton.Begin();
    }

    private bool EndEffect() => Dead || ClickedAgain;
}