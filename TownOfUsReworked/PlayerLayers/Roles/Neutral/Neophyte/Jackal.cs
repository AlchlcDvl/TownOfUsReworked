namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Jackal : Neophyte
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number RecruitCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool JackalVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool RecruitVent { get; set; } = false;

    public PlayerControl Recruit1 { get; set; }
    public PlayerControl Recruit2 { get; set; }
    public PlayerControl Recruit3 { get; set; }
    public CustomButton RecruitButton { get; set; }
    public CustomButton KillButton { get; set; }
    public bool RecruitsDead => !Recruit2 || !Recruit1 || (!Recruit3 && Recruit1 && Recruit2 && Recruit1.HasDied() && Recruit2.HasDied());
    public bool AllRecruitsDead => Recruit1 && Recruit1.HasDied() && Recruit2 && Recruit2.HasDied() && Recruit3 && Recruit3.HasDied();

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Jackal : CustomColorManager.Neutral;
    public override string Name => "Jackal";
    public override LayerEnum Type => LayerEnum.Jackal;
    public override Func<string> StartText => () => "Gain A Majority";
    public override Func<string> Description => () => "- You start off with 2 recruits. 1 of them is always <color=#8CFFFFFF>Crew</color>\nand the other is either a <color=#008000FF>" +
        "Syndicate</color>, <color=#FF1919FF>Intruder</color> or a <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killer</color>\n- When both recruits die, you can recruit a third" +
        " member into the <color=#575657FF>Cabal</color>";
    public override AttackEnum AttackVal => AttackEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Recruit or kill anyone who can oppose the <color=#575657FF>Cabal</color>";
        SubFaction = SubFaction.Cabal;
        RecruitButton ??= new(this, new SpriteName("Recruit"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Recruit, (PlayerBodyExclusion)Exception, "RECRUIT",
            (UsableFunc)Usable1, new Cooldown(RecruitCd));
        KillButton ??= new(this, new SpriteName("JackalKill"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Kill, (PlayerBodyExclusion)Exception, "KILL", (UsableFunc)Usable2,
            new Cooldown(RecruitCd));
    }

    public override void Deinit()
    {
        base.Deinit();
        Recruit1 = null;
        Recruit2 = null;
        Recruit3 = null;
    }

    public void Recruit(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            RoleGen.RpcConvert(target.PlayerId, Player.PlayerId, SubFaction.Cabal);

        RecruitButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => Members.Contains(player.PlayerId);

    public void Kill(PlayerControl target) => KillButton.StartCooldown(Interact(Player, target, true));

    public bool Usable1() => RecruitsDead;

    public bool Usable2() => AllRecruitsDead;

    public IEnumerable<PlayerControl> GetOtherRecruits(PlayerControl recruit) => Members.Where(x => x != recruit.PlayerId).Select(PlayerById);
}