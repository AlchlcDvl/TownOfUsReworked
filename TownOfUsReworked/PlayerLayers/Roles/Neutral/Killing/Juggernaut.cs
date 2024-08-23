namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu2.LayerSubOptions)]
public class Juggernaut : Neutral
{
    [NumberOption(MultiMenu2.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static float AssaultCd { get; set; } = 25f;

    [NumberOption(MultiMenu2.LayerSubOptions, 2.5f, 30f, 2.5f, Format.Time)]
    public static float AssaultBonus { get; set; } = 5f;

    [ToggleOption(MultiMenu2.LayerSubOptions)]
    public static bool JuggVent { get; set; } = false;

    public int JuggKills { get; set; }
    public CustomButton AssaultButton { get; set; }

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Juggernaut : CustomColorManager.Neutral;
    public override string Name => "Juggernaut";
    public override LayerEnum Type => LayerEnum.Juggernaut;
    public override Func<string> StartText => () => "Your Power Grows With Every Kill";
    public override Func<string> Description => () => "- With each kill, your kill cooldown decreases" + (JuggKills >= 4 ? "\n- You can bypass all forms of protection" : "");
    public override AttackEnum AttackVal => (AttackEnum)Mathf.Clamp(JuggKills, 1, 3);
    public override DefenseEnum DefenseVal => JuggKills >= 3 ? DefenseEnum.Basic : DefenseEnum.None;

    public override void Init()
    {
        BaseStart();
        Objectives = () => "- Assault anyone who can oppose you";
        Alignment = Alignment.NeutralKill;
        JuggKills = 0;
        AssaultButton = CreateButton(this, new SpriteName("Assault"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Assault, new Cooldown(AssaultCd), (DifferenceFunc)Difference,
            (PlayerBodyExclusion)Exception, "ASSAULT");
    }

    public void Assault()
    {
        var cooldown = Interact(Player, AssaultButton.TargetPlayer, true, false, JuggKills >= 4);

        if (cooldown != CooldownType.Fail)
        {
            JuggKills++;
            Flash(Color);
        }

        AssaultButton.StartCooldown(cooldown);
    }

    public bool Exception(PlayerControl player) => (player.Is(SubFaction) && SubFaction != SubFaction.None) || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate) ||
        Player.IsLinkedTo(player);

    public float Difference() => -(AssaultBonus * JuggKills);
}