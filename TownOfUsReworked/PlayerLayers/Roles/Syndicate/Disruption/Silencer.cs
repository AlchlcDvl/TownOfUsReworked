namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Silencer)]
public sealed class Silencer : Syndicate, IIntimidator
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
    public PlayerControl Target { get; private set; }

    protected override UColor MainColor => CustomColorManager.Silencer;
    public override LayerEnum Type => LayerEnum.Silencer;
    public override Func<string> StartText { get; } = () => "You Are The One Who Hushes";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say" + (SilenceRevealed ? "\n- Everyone will be alerted at the "  +
        "start of the meeting that someone has been silenced " : "") + (WhispersNotPrivateS ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Disruption;
        Target = null;
        SilenceButton ??= new(this, new SpriteName("Silence"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Silence, new Cooldown(SilenceCd), "SILENCE",
            (PlayerBodyExclusion)Exception1);
    }

    public override void Reset(bool meeting, bool start) => Target = null;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Target == player)
            name += " <#AAB43EFF>乂</color>";
    }

    private void Silence(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Target = target;
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, Target);

            if (target.IsBlackmailed())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        SilenceButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => player == Target || (!SilenceMates && ((player.Is(Faction) && Faction is not (Faction.Crew or Faction.Neutral) && !SilenceMates) ||
        (player.Is(SubFaction) && SubFaction != SubFaction.None)));

    public override void ReadRPC(NetData reader) => Target = reader.ReadPlayer();
}