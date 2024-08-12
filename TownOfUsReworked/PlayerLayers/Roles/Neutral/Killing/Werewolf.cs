namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Werewolf : Neutral
{
    public bool CanMaul => Rounds is not (0 or 2) || CustomGameOptions.CanStillAttack;
    public CustomButton MaulButton { get; set; }
    public int Rounds { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Werewolf : CustomColorManager.Neutral;
    public override string Name => "Werewolf";
    public override LayerEnum Type => LayerEnum.Werewolf;
    public override Func<string> StartText => () => "AWOOOOOOOOOO";
    public override Func<string> Description => () => $"- You kill everyone within {CustomGameOptions.MaulRadius}m";
    public override AttackEnum AttackVal => AttackEnum.Powerful;
    public override DefenseEnum DefenseVal => CanMaul ? DefenseEnum.None : DefenseEnum.Basic;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Maul anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        MaulButton = CreateButton(this, new SpriteName("Maul"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)HitMaul, new Cooldown(CustomGameOptions.MaulCd), "MAUL",
            (PlayerBodyExclusion)Exception, (UsableFunc)Usable);
        Data.Role.IntroSound = GetAudio("WerewolfIntro");
    }

    public void Maul()
    {
        foreach (var player in GetClosestPlayers(Player.transform.position, CustomGameOptions.MaulRadius))
        {
            Spread(Player, player);

            if (CanAttack(AttackVal, player.GetDefenseValue()))
                RpcMurderPlayer(Player, player, DeathReasonEnum.Mauled, false);
        }
    }

    public void HitMaul()
    {
        var cooldown = Interact(Player, MaulButton.TargetPlayer, true);

        if (cooldown != CooldownType.Fail)
            Maul();

        MaulButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public bool Usable() => CanMaul;
}