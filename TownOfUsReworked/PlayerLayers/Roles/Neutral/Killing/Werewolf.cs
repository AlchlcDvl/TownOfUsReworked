namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Werewolf)]
public sealed class Werewolf : NKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number MaulCd = 25;

    [ToggleOption]
    private static bool CanStillAttack = false;

    [StringOption<WerewolfVentOptions>]
    private static WerewolfVentOptions WerewolfVent = WerewolfVentOptions.Always;

    private bool CanMaul => Rounds is not (0 or 2) || CanStillAttack;
    private CustomButton MaulButton { get; set; }
    public int Rounds { get; set; }

    public override UColor MainColor => CustomColorManager.Werewolf;
    public override LayerEnum Type => LayerEnum.Werewolf;
    public override Func<string> StartText => () => "AWOOOOOOOOOO";
    public override Func<string> Description => () => $"- You kill everyone within {GameSettings.InteractionDistance}m";
    public override AttackEnum AttackVal => AttackEnum.Powerful;
    public override DefenseEnum DefenseVal => CanMaul ? DefenseEnum.None : DefenseEnum.Basic;
    public override bool CanVent => base.CanVent && (WerewolfVent == 0 || (CanMaul && (int)WerewolfVent == 1) || (!CanMaul && (int)Werewolf.WerewolfVent == 2));

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Maul anyone who can oppose you";
        MaulButton ??= new(this, new SpriteName("Maul"), AbilityTypes.Targetless, KeybindType.ActionSecondary, (OnClickTargetless)Maul, new Cooldown(MaulCd), "MAUL", (UsableFunc)Usable,
            (PlayerBodyExclusion)Exception, (ConditionFunc)Condition);
    }

    private void Maul()
    {
        GetClosestPlayers(Player, GameSettings.InteractionDistance).ForEach(x => Interact(Player, x, true, lunge: false));
        MaulButton.StartCooldown();
    }

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    private bool Usable() => CanMaul;

    private bool Condition() => GetClosestPlayers(Player, GameSettings.InteractionDistance).Any();
}