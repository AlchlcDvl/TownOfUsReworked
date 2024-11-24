namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Mystic : Crew
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number RevealCd { get; set; } = new(25);

    private bool ConvertedDead => !AllPlayers().Any(x => !x.HasDied() && !x.Is(SubFaction.None) && !x.Is(SubFaction));
    private CustomButton RevealButton { get; set; }

    public override UColor Color => ClientOptions.CustomCrewColors ? CustomColorManager.Mystic : CustomColorManager.Crew;
    public override string Name => "Mystic";
    public override LayerEnum Type => LayerEnum.Mystic;
    public override Func<string> StartText => () => "You Know When Converts Happen";
    public override Func<string> Description => () => "- You can investigate players to see if they have been converted\n- Whenever someone has been converted, you will be alerted to it\n-" +
        " When all converted and converters die, you will become a <color=#71368AFF>Seer</color>";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.CrewAudit;
        RevealButton ??= new(this, "REVEAL", new SpriteName("MysticReveal"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClickPlayer)Reveal, (PlayerBodyExclusion)Exception,
            new Cooldown(RevealCd));
    }

    public void TurnSeer() => new Seer().RoleUpdate(this, Player);

    public override void UpdatePlayer()
    {
        if (ConvertedDead && !Dead)
            TurnSeer();
    }

    private void Reveal(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
            Flash((!target.Is(SubFaction) && SubFaction != SubFaction.None && !target.Is(Alignment.NeutralNeo)) || target.IsFramed() ? UColor.red : UColor.green);

        RevealButton.StartCooldown(cooldown);
    }

    private bool Exception(PlayerControl player) => player.Is(SubFaction) && SubFaction != SubFaction.None;
}