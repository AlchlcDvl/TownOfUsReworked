namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Juggernaut : NKilling
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number AssaultCd = 25;

    [NumberOption(2.5f, 30f, 2.5f, Format.Time)]
    public static Number AssaultBonus = 5;

    [ToggleOption]
    public static bool JuggVent = false;

    public int JuggKills { get; set; }
    public CustomButton AssaultButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Juggernaut : FactionColor;
    public override LayerEnum Type => LayerEnum.Juggernaut;
    public override Func<string> StartText => () => "Your Power Grows With Every Kill";
    public override Func<string> Description => () => "- With each kill, your kill cooldown decreases" + (JuggKills >= 4 ? "\n- You can bypass all forms of protection" : "");
    public override AttackEnum AttackVal => (AttackEnum)Mathf.Clamp(JuggKills, 1, 3);
    public override DefenseEnum DefenseVal => JuggKills >= 3 ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        base.Init();
        Objectives = () => "- Assault anyone who can oppose you";
        JuggKills = 0;
        AssaultButton ??= new(this, new SpriteName("Assault"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Assault, new Cooldown(AssaultCd), (DifferenceFunc)Difference,
            (PlayerBodyExclusion)Exception, "ASSAULT");
    }

    public void Assault(PlayerControl target)
    {
        var cooldown = Interact(Player, target, true, bypass: JuggKills >= 4);

        if (cooldown != CooldownType.Fail)
            JuggKills++;

        if (JuggKills == 4)
            Flash(Color);

        AssaultButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public float Difference() => -(AssaultBonus * JuggKills);
}