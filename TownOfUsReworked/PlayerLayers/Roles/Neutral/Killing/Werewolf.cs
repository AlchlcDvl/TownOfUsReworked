namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Werewolf : NKilling
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number MaulCd { get; set; } = new(25);

    [NumberOption(MultiMenu.LayerSubOptions, 0.5f, 5f, 0.25f, Format.Distance)]
    public static Number MaulRadius { get; set; } = new(1.5f);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool CanStillAttack { get; set; } = false;

    [StringOption(MultiMenu.LayerSubOptions)]
    public static WerewolfVentOptions WerewolfVent { get; set; } = WerewolfVentOptions.Always;

    public bool CanMaul => Rounds is not (0 or 2) || CanStillAttack;
    public CustomButton MaulButton { get; set; }
    public int Rounds { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Werewolf : FactionColor;
    public override string Name => "Werewolf";
    public override LayerEnum Type => LayerEnum.Werewolf;
    public override Func<string> StartText => () => "AWOOOOOOOOOO";
    public override Func<string> Description => () => $"- You kill everyone within {MaulRadius}m";
    public override AttackEnum AttackVal => AttackEnum.Powerful;
    public override DefenseEnum DefenseVal => CanMaul ? DefenseEnum.None : DefenseEnum.Basic;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Maul anyone who can oppose you";
        MaulButton ??= new(this, new SpriteName("Maul"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Maul, new Cooldown(MaulCd), "MAUL", (UsableFunc)Usable,
            (PlayerBodyExclusion)Exception);
    }

    public void Maul(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true);

        if (cooldown != CooldownType.Fail)
        {
            foreach (var player in GetClosestPlayers(Player, MaulRadius))
            {
                Spread(Player, player);

                if (CanAttack(AttackVal, player.GetDefenseValue()))
                    RpcMurderPlayer(Player, player, DeathReasonEnum.Mauled, false);
            }
        }

        MaulButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public bool Usable() => CanMaul;
}