namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class SerialKiller : NKilling
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number BloodlustCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static Number BloodlustDur { get; set; } = new(10);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 15f, 0.5f, Format.Time)]
    public static Number StabCd { get; set; } = new(5);

    [StringOption(MultiMenu.LayerSubOptions)]
    public static SKVentOptions SKVentOptions { get; set; } = SKVentOptions.Always;

    public CustomButton BloodlustButton { get; set; }
    public CustomButton StabButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.SerialKiller : CustomColorManager.Neutral;
    public override string Name => "Serial Killer";
    public override LayerEnum Type => LayerEnum.SerialKiller;
    public override Func<string> StartText => () => "You Like To Play With Knives";
    public override Func<string> Description => () => "- You can go into bloodlust\n- When in bloodlust, your kill cooldown is very short\n- If and when an <#803333FF>Escort</color>," +
        " <#801780FF>Consort</color> or <#00FF00FF>Glitch</color> tries to block you, you will immediately kill them, regardless of your cooldown\n- You are immune to roleblocks";
    public override AttackEnum AttackVal => AttackEnum.Powerful;
    public override DefenseEnum DefenseVal => BloodlustButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;
    public override bool RoleBlockImmune => true;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Stab anyone who can oppose you";
        BloodlustButton ??= new(this, new SpriteName("Bloodlust"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClickTargetless)Lust, new Cooldown(BloodlustCd), (EndFunc)EndEffect,
            new Duration(BloodlustDur), "BLOODLUST");
        StabButton ??= new(this, new SpriteName("Stab"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Stab, new Cooldown(StabCd), "STAB", (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
    }

    public void Lust()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BloodlustButton);
        BloodlustButton.Begin();
    }

    public void Stab(PlayerControl target) => StabButton.StartCooldown(Interact(Player, target, true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public bool Usable() => BloodlustButton.EffectActive;

    public bool EndEffect() => Dead;
}