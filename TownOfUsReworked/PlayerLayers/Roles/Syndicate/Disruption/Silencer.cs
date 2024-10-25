namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Silencer : Syndicate
{
    [NumberOption(MultiMenu.LayerSubOptions, 10f, 60f, 2.5f, Format.Time)]
    public static Number SilenceCd { get; set; } = new(25);

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool WhispersNotPrivateS { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SilenceMates { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool SilenceRevealed { get; set; } = true;

    public CustomButton SilenceButton { get; set; }
    public PlayerControl SilencedPlayer { get; set; }
    public bool ShookAlready { get; set; }
    public Sprite PrevOverlay { get; set; }
    public UColor? PrevColor { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Silencer : CustomColorManager.Syndicate;
    public override string Name => "Silencer";
    public override LayerEnum Type => LayerEnum.Silencer;
    public override Func<string> StartText => () => "You Are The One Who Hushes";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say" + (SilenceRevealed ? "\n- Everyone will be alerted at the "  +
        "start of the meeting that someone has been silenced " : "") + (WhispersNotPrivateS ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";

    public override void Init()
    {
        BaseStart();
        Alignment = Alignment.SyndicateDisrup;
        SilencedPlayer = null;
        SilenceButton ??= CreateButton(this, new SpriteName("Silence"), AbilityType.Alive, KeybindType.Secondary, (OnClick)Silence, new Cooldown(SilenceCd), "SILENCE",
            (PlayerBodyExclusion)Exception1);
    }

    public void Silence()
    {
        var cooldown = Interact(Player, SilenceButton.GetTarget<PlayerControl>());

        if (cooldown != CooldownType.Fail)
        {
            SilencedPlayer = SilenceButton.GetTarget<PlayerControl>();
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, SilencedPlayer);
        }

        SilenceButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !SilenceMates);

    public override void ReadRPC(MessageReader reader) => SilencedPlayer = reader.ReadPlayer();
}