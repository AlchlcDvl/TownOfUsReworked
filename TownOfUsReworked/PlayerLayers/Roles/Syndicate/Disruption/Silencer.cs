namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public sealed class Silencer : Syndicate, ISilencer
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number SilenceCd = 25;

    [ToggleOption]
    public static bool WhispersNotPrivateS = true;

    [ToggleOption]
    public static bool SilenceMates = false;

    [ToggleOption]
    public static bool SilenceRevealed = true;

    private CustomButton SilenceButton { get; set; }
    public bool ShookAlready { get; set; }
    public PlayerControl Target => SilencedPlayer;
    public PlayerControl SilencedPlayer { get; set; }

    public override UColor Color => ClientOptions.CustomSynColors ? CustomColorManager.Silencer : FactionColor;
    public override LayerEnum Type => LayerEnum.Silencer;
    public override Func<string> StartText => () => "You Are The One Who Hushes";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say" + (SilenceRevealed ? "\n- Everyone will be alerted at the "  +
        "start of the meeting that someone has been silenced " : "") + (WhispersNotPrivateS ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        SilencedPlayer = null;
        SilenceButton ??= new(this, new SpriteName("Silence"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Silence, new Cooldown(SilenceCd), "SILENCE",
            (PlayerBodyExclusion)Exception1);
    }

    public override void Reset(bool meeting, bool start) => SilencedPlayer = null;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (SilencedPlayer == player)
            name += " <#AAB43EFF>乂</color>";
    }

    private void Silence(PlayerControl target)
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

    private bool Exception1(PlayerControl player) => player == SilencedPlayer || (player.Is(Faction) && Faction is Faction.Intruder or Faction.Syndicate && !SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None && !SilenceMates);

    public override void ReadRPC(MessageReader reader) => SilencedPlayer = reader.ReadPlayer();
}