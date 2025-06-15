namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Wraith)]
public sealed class Wraith : Deception
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number InvisCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number InvisDur = 10;

    [ToggleOption]
    private static bool WraithVent = false;

    private CustomButton InvisButton;
    private bool ClickedAgain;

    protected override UColor MainColor => CustomColorManager.Wraith;
    public override Layer Type => Layer.Wraith;
    public override string StartText => "Sneaky Sneaky";
    public override string Description => $"- You can turn invisible\n{CommonAbilities}";
    public override bool CanVent => WraithVent;

    public override void Init()
    {
        base.Init();
        InvisButton ??= new(this, "INVISIBILITY", new SpriteName("Invis"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)HitInvis, new Cooldown(InvisCd), (EffectStartVoid)Invis,
            new Duration(InvisDur), (EffectEndVoid)UnInvis, (EndFunc)EndEffect, (ClickedAgainVoid)ClickAgain);
    }

    private void Invis() => MiscUtils.Invis(Player, InvisDur, EndEffect, Player.IsBuddyWith(LocalPlayer, Faction));

    private void UnInvis() => ClickedAgain = false;

    private void ClickAgain() => ClickedAgain = true;

    private void HitInvis()
    {
        CallRpc(ReworkedRpc.Action, ActionsRpc.ButtonAction, InvisButton);
        InvisButton.Begin();
    }

    private bool EndEffect() => Dead || ClickedAgain;
}