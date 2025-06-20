namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.SerialKiller)]
public sealed class SerialKiller : OKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number BloodlustCd = 25;

    [NumberOption(5f, 30f, 1f, Format.Time)]
    private static Number BloodlustDur = 10;

    [NumberOption(0.5f, 15f, 0.5f, Format.Time)]
    private static Number StabCd = 5;

    [StringOption<SkVentOptions>]
    private static SkVentOptions SkVentOptions = SkVentOptions.Always;

    public CustomButton BloodlustButton;
    private CustomButton StabButton;

    protected override UColor MainColor => CustomColorManager.SerialKiller;
    public override Layer Type => Layer.SerialKiller;
    public override string StartText => "You Like To Play With Knives";
    public override string Description => "- You can go into bloodlust\n- When in bloodlust, your kill cooldown is very short\n- If and when an <#803333FF>Escort</color>," +
        " <#801780FF>Consort</color> or <#00FF00FF>Glitch</color> tries to block you, you will immediately kill them, regardless of your cooldown\n- You are immune to roleblocks";
    public override Attack Attack => Attack.Powerful;
    public override Defense Defense => BloodlustButton.EffectActive ? Defense.Basic : Defense.None;
    public override bool RoleBlockImmune => true;
    public override bool CanVent => base.CanVent && (SkVentOptions == 0 || (BloodlustButton.EffectActive && (int)SkVentOptions == 1) || (!BloodlustButton.EffectActive && (int)SkVentOptions == 2));
    protected override Faction ActualFaction => Faction.SerialKiller;

    public override void Init()
    {
        Objectives = () => "- Stab anyone who can oppose you";
        BloodlustButton ??= new(this, new SpriteName("Bloodlust"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Lust, new Cooldown(BloodlustCd), (EndFunc)EndEffect,
            new Duration(BloodlustDur), "BLOODLUST");
        StabButton ??= new(this, new SpriteName("Stab"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Stab, new Cooldown(StabCd), "STAB", (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
    }

    private void Lust() => BloodlustButton.TriggerRpcAndBegin();

    private void Stab(PlayerControl target) => StabButton.StartCooldown(Interact(Player, target, true));

    private bool Exception(PlayerControl player) => Player.IsBuddyWith(player, Handler.CurrentFaction);

    private bool Usable() => BloodlustButton.EffectActive;

    private bool EndEffect() => Dead;
}