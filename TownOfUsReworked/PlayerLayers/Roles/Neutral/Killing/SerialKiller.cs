namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class SerialKiller : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float BloodlustCd { get; set; } = 25f;

    [NumberOption(MultiMenu.LayerSubOptions, 5f, 30f, 1f, Format.Time)]
    public static float BloodlustDur { get; set; } = 10f;

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 15f, 0.5f, Format.Time)]
    public static float StabCd { get; set; } = 5f;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static SKVentOptions SKVentOptions { get; set; } = SKVentOptions.Always;

    public CustomButton BloodlustButton { get; set; }
    public CustomButton StabButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.SerialKiller : CustomColorManager.Neutral;
    public override string Name => "Serial Killer";
    public override LayerEnum Type => LayerEnum.SerialKiller;
    public override Func<string> StartText => () => "You Like To Play With Knives";
    public override Func<string> Description => () => "- You can go into bloodlust\n- When in bloodlust, your kill cooldown is very short\n- If and when an <color=#803333FF>Escort</color>," +
        " <color=#801780FF>Consort</color> or <color=#00FF00FF>Glitch</color> tries to block you, you will immediately kill them, regardless of your cooldown\n- You are immune to roleblocks";
    public override AttackEnum AttackVal => AttackEnum.Powerful;
    public override DefenseEnum DefenseVal => BloodlustButton.EffectActive ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Stab anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        RoleBlockImmune = true;
        BloodlustButton = CreateButton(this, new SpriteName("Bloodlust"), AbilityTypes.Targetless, KeybindType.Secondary, (OnClick)Lust, new Cooldown(BloodlustCd), new Duration(BloodlustDur),
            "BLOODLUST", (EndFunc)EndEffect);
        StabButton = CreateButton(this, new SpriteName("Stab"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Stab, new Cooldown(StabCd), "STAB", (PlayerBodyExclusion)Exception,
            (UsableFunc)Usable);
    }

    public void Lust()
    {
        CallRpc(CustomRPC.Action, ActionsRPC.ButtonAction, BloodlustButton);
        BloodlustButton.Begin();
    }

    public void Stab() => StabButton.StartCooldown(Interact(Player, StabButton.TargetPlayer, true));

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public bool Usable() => BloodlustButton.EffectActive;

    public bool EndEffect() => Dead;
}