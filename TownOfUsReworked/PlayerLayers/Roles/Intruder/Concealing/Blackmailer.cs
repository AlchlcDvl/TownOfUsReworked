namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Blackmailer)]
public sealed class Blackmailer : Concealing, IIntimidator
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    private static Number BlackmailCd = 25;

    [ToggleOption]
    public static bool WhispersNotPrivateB = true;

    [ToggleOption]
    private static bool BlackmailMates = false;

    [ToggleOption]
    public static bool BmRevealed = true;

    private CustomButton BlackmailButton;
    public bool ShookAlready { get; set; }
    public PlayerControl Target { get; private set; }

    protected override UColor MainColor => CustomColorManager.Blackmailer;
    public override Layer Type => Layer.Blackmailer;
    public override string StartText => "You Know Their Secrets";
    public override string Description => "- You can silence players to ensure they cannot hear what others say\n" + (BmRevealed ? ("- Everyone will be alerted at the start " +
        "of the meeting that someone has been silenced ") : "") + (WhispersNotPrivateB ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";

    public override void Init()
    {
        base.Init();
        Target = null;
        BlackmailButton ??= new(this, "BLACKMAIL", new SpriteName("Blackmail"), AbilityTypes.Player, KeybindType.Secondary, (OnClickPlayer)Blackmail, new Cooldown(BlackmailCd),
            (PlayerBodyExclusion)Exception1);
    }

    public override void Reset(bool meeting, bool start) => Target = null;

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (Target == player)
            name += " <#02A752FF>Φ</color>";
    }

    private void Blackmail(PlayerControl target)
    {
        var cooldown = Interact(Player, target);

        if (cooldown != CooldownType.Fail)
        {
            Target = target;
            PerformRpcAction(Target);

            if (target.IsSilenced())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => player == Target || (!BlackmailMates && Player.IsBuddyWith(player, Handler.CurrentFaction));

    public override void ReadRPC(RpcReader reader) => Target = reader.ReadPlayer();
}