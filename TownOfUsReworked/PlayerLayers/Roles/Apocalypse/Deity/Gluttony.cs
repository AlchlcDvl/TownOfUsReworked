namespace TownOfUsReworked.PlayerLayers.Roles;

// TODO: Finish this
[LayerHeaderOption(Layer.Gluttony)]
public sealed class Gluttony : Deity
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number HungerCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number HungerDur = 10;

    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number ConsumeCd = 25;

    [ToggleOption]
    private static bool GlutVent = true;

    private CustomButton HungerButton;
    private CustomButton ConsumeButton;

    protected override UColor MainColor => CustomColorManager.Gluttony;
    public override Layer Type => Layer.Gluttony;
    public override string Description => "- You can initiate an insatiable hunger into people, causing them to be unable to use their abilities" + CommonAbilities;
    public override bool CanVent => base.CanVent && GlutVent;
    public override Attack Attack => Attack.Powerful;

    public override void Init()
    {
        base.Init();
        ConsumeButton ??= new(this, new SpriteName("Consume"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Consume, "CONSUME", new Cooldown(ConsumeCd),
            (PlayerBodyExclusion)Exception);
        HungerButton ??= new(this, new SpriteName("Hunger"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)CauseHunger, "HUNGER", new Cooldown(HungerCd),
            new Duration(HungerDur));
    }

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private void Consume(PlayerControl target) => ConsumeButton.StartCooldown(Interact(Player, target, true));

    private void CauseHunger()
    {
        HungerButton.StartCooldown();
    }
}