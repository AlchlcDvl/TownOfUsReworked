namespace TownOfUsReworked.PlayerLayers.Roles;

public class Jackal : Neutral
{
    public PlayerControl EvilRecruit { get; set; }
    public PlayerControl GoodRecruit { get; set; }
    public PlayerControl BackupRecruit { get; set; }
    public CustomButton RecruitButton { get; set; }
    public bool RecruitsDead => EvilRecruit == null || GoodRecruit == null || (BackupRecruit == null && GoodRecruit != null && EvilRecruit != null && GoodRecruit.HasDied() &&
        EvilRecruit.HasDied());
    public List<byte> Recruited { get; set; }

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Jackal : CustomColorManager.Neutral;
    public override string Name => "Jackal";
    public override LayerEnum Type => LayerEnum.Jackal;
    public override Func<string> StartText => () => "Gain A Majority";
    public override Func<string> Description => () => "- You start off with 2 recruits. 1 of them is always <color=#8CFFFFFF>Crew</color>\nand the other is either a <color=#008000FF>" +
        "Syndicate</color>, <color=#FF1919FF>Intruder</color> or a <color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killer</color>\n- When both recruits die, you can recruit a third" +
        " member into the <color=#575657FF>Cabal</color>";

    public Jackal(PlayerControl player) : base(player)
    {
        Objectives = () => "- Recruit or kill anyone who can oppose the <color=#575657FF>Cabal</color>";
        SubFaction = SubFaction.Cabal;
        SubFactionColor = CustomColorManager.Cabal;
        Alignment = Alignment.NeutralNeo;
        Recruited = new() { Player.PlayerId };
        RecruitButton = new(this, "Recruit", AbilityTypes.Alive, "ActionSecondary", Recruit, Exception);
        SubFactionSymbol = "$";
        player.Data.Role.IntroSound = GetAudio("JackalIntro");
    }

    public void Recruit()
    {
        var cooldown = Interact(Player, RecruitButton.TargetPlayer);

        if (cooldown != CooldownType.Fail)
            RoleGen.RpcConvert(RecruitButton.TargetPlayer.PlayerId, Player.PlayerId, SubFaction.Cabal);

        RecruitButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => Recruited.Contains(player.PlayerId);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        RecruitButton.Update2("RECRUIT", RecruitsDead);
    }
}