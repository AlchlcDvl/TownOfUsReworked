namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Silencer)]
public sealed class Silencer : Disruption, IIntimidator
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number SilenceCd = 25;

    [ToggleOption]
    public static bool WhispersNotPrivateS = true;

    [ToggleOption]
    private static bool SilenceMates = false;

    [ToggleOption]
    public static bool SilenceRevealed = true;

    private CustomButton SilenceButton;
    public bool ShookAlready { get; set; }
    public PlayerControl Target { get; private set; }

    protected override UColor MainColor => CustomColorManager.Silencer;
    public override Layer Type => Layer.Silencer;
    public override string StartText => "You Are The One Who Hushes";
    public override string Description => "- You can silence players to ensure they cannot hear what others say" + (SilenceRevealed ? "\n- Everyone will be alerted at the "  +
        "start of the meeting that someone has been silenced " : string.Empty) + (WhispersNotPrivateS ? "\n- You can read whispers during meetings" : string.Empty) + $"\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Target = null;
        SilenceButton ??= new(this, new SpriteName("Silence"), ReworkedAbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Silence, new Cooldown(SilenceCd), "SILENCE",
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
            PerformRpcAction(Target);

            if (target.IsBlackmailed())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        SilenceButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => player == Target || (!SilenceMates && Player.IsBuddyWith(player, Handler.CurrentFaction));

    public override void ReadRPC(RpcReader reader) => Target = reader.ReadPlayer();
}