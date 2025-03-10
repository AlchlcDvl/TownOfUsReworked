namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Juggernaut)]
public sealed class Juggernaut : NKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number AssaultCd = 25;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    private static Number AssaultBonus = 5;

    [ToggleOption]
    public static bool JuggVent = false;

    private CustomButton AssaultButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Juggernaut : FactionColor;
    public override LayerEnum Type => LayerEnum.Juggernaut;
    public override Func<string> StartText => () => "Your Power Grows With Every Kill";
    public override Func<string> Description => () => "- With each kill, your kill cooldown decreases" + (KillCounts.GetValueOrDefault(PlayerId) >= 4 ? "\n- You can bypass all forms of protection" : "");
    public override AttackEnum AttackVal => (AttackEnum)Mathf.Clamp(KillCounts.GetValueOrDefault(PlayerId), 1, 3);
    public override DefenseEnum DefenseVal => KillCounts.GetValueOrDefault(PlayerId) >= 3 ? DefenseEnum.Basic : DefenseEnum.None;

    protected override void Init()
    {
        base.Init();
        Objectives = () => "- Assault anyone who can oppose you";
        AssaultButton ??= new(this, new SpriteName("Assault"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Assault, new Cooldown(AssaultCd), (DifferenceFunc)Difference,
            (PlayerBodyExclusion)Exception, "ASSAULT");
    }

    private void Assault(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true, bypass: KillCounts.GetValueOrDefault(PlayerId) >= 4);

        if (KillCounts.GetValueOrDefault(PlayerId) == 4)
            Flash(Color);

        AssaultButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    private float Difference() => -(AssaultBonus * KillCounts.GetValueOrDefault(PlayerId));
}