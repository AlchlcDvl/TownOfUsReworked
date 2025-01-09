namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Silencer : Syndicate, ISilencer
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
    public bool ShookAlready { get; set; }
    public PlayerControl Target { get; set; }
    public PlayerControl SilencedPlayer
    {
        get => Target;
        set => Target = value;
    }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Silencer : FactionColor;
    public override string Name => "Silencer";
    public override LayerEnum Type => LayerEnum.Silencer;
    public override Func<string> StartText => () => "You Are The One Who Hushes";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say" + (SilenceRevealed ? "\n- Everyone will be alerted at the "  +
        "start of the meeting that someone has been silenced " : "") + (WhispersNotPrivateS ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Alignment = Alignment.SyndicateDisrup;
        SilencedPlayer = null;
        SilenceButton ??= new(this, new SpriteName("Silence"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Silence, new Cooldown(SilenceCd), "SILENCE",
            (PlayerBodyExclusion)Exception1);
    }

    public void Silence(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            SilencedPlayer = target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, SilencedPlayer);

            if (target.IsBlackmailed())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        SilenceButton.StartCooldown(cooldown);
    }

    public bool Exception1(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !SilenceMates);

    public override void ReadRPC(MessageReader reader) => SilencedPlayer = reader.ReadPlayer();
}