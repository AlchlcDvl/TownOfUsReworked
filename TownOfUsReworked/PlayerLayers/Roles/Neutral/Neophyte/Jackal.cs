namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Jackal : Neutral
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float RecruitCd { get; set; } = 25f;

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
    public List<byte> Recruited { get; set; }

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
        BaseStart();
        Objectives = () => "- Recruit or kill anyone who can oppose the <color=#575657FF>Cabal</color>";
        SubFaction = SubFaction.Cabal;
        SubFactionColor = CustomColorManager.Cabal;
        Alignment = Alignment.NeutralNeo;
        Recruited = [ Player.PlayerId ];
        RecruitButton = CreateButton(this, new SpriteName("Recruit"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Recruit, (PlayerBodyExclusion)Exception, "RECRUIT",
            (UsableFunc)Usable1, new Cooldown(RecruitCd));
        KillButton = CreateButton(this, new SpriteName("JackalKill"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Kill, (PlayerBodyExclusion)Exception, "KILL",
            (UsableFunc)Usable2, new Cooldown(RecruitCd));
        Data.Role.IntroSound = GetAudio("JackalIntro");
    }

    public void Recruit()
    {
        var cooldown = Interact(Player, RecruitButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            RoleGen.RpcConvert(RecruitButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Cabal);

        RecruitButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => Recruited.Contains(player.PlayerId);

    public void Kill() => KillButton.StartCooldown(Interact(Player, KillButton.TargetPlayer, true));

    public bool Usable1() => RecruitsDead;

    public bool Usable2() => AllRecruitsDead;
}