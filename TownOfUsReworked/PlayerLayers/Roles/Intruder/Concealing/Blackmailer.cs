namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Blackmailer : Intruder, IBlackmailer
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number BlackmailCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WhispersNotPrivateB { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BlackmailMates { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool BMRevealed { get; set; } = true;

    public CustomButton BlackmailButton { get; set; }
    public bool ShookAlready { get; set; }
    public PlayerControl Target { get; set; }
    public PlayerControl BlackmailedPlayer
    {
        get => Target;
        set => Target = value;
    }

    public override UColor Color => ClientOptions.CustomIntColors ? CustomColorManager.Blackmailer : FactionColor;
    public override string Name => "Blackmailer";
    public override LayerEnum Type => LayerEnum.Blackmailer;
    public override Func<string> StartText => () => "You Know Their Secrets";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say\n" + (BMRevealed ? ("- Everyone will be alerted at the start " +
        "of the meeting that someone has been silenced ") : "") + (WhispersNotPrivateB ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.IntruderConceal;
        BlackmailedPlayer = null;
        BlackmailButton ??= new(this, "BLACKMAIL", new SpriteName("Blackmail"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Blackmail, new Cooldown(BlackmailCd),
            (PlayerBodyExclusion)Exception1);
    }

    public void Blackmail(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            BlackmailedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, BlackmailedPlayer);
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == BlackmailedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !BlackmailMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !BlackmailMates);

    public override void ReadRPC(MessageReader reader) => BlackmailedPlayer = reader.ReadPlayer();
}