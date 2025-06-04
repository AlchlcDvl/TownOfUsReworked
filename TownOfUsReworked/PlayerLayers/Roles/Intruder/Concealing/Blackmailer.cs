namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Blackmailer)]
public sealed class Blackmailer : Intruder, IIntimidator
{
    [NumberOption(10f, 60f, 2.5f, Format.Time)]
    public static Number BlackmailCd = 25;

    [ToggleOption]
    public static bool WhispersNotPrivateB = true;

    [ToggleOption]
    public static bool BlackmailMates = false;

    [ToggleOption]
    public static bool BmRevealed = true;

    private CustomButton BlackmailButton { get; set; }
    public bool ShookAlready { get; set; }
    public PlayerControl Target { get; private set; }

    protected override UColor MainColor => CustomColorManager.Blackmailer;
    public override LayerEnum Type => LayerEnum.Blackmailer;
    public override Func<string> StartText { get; } = () => "You Know Their Secrets";
    public override Func<string> Description => () => "- You can silence players to ensure they cannot hear what others say\n" + (BmRevealed ? ("- Everyone will be alerted at the start " +
        "of the meeting that someone has been silenced ") : "") + (WhispersNotPrivateB ? "\n- You can read whispers during meetings" : "") + $"\n{CommonAbilities}";

    protected override void Init()
    {
        base.Init();
        Alignment = Alignment.Concealing;
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
            CallRpc(CustomRPC.Action, ActionsRPC.LayerAction, this, Target);

            if (target.IsSilenced())
                CustomAchievementManager.UnlockAchievement("EerieSilence");
        }

        BlackmailButton.StartCooldown(cooldown);
    }

    private bool Exception1(PlayerControl player) => player == Target || (((player.Is(Faction) && Faction.IsFactionedEvil()) || Player.IsLinkedTo(player)) && !BlackmailMates);

    public override void ReadRPC(NetData reader) => Target = reader.ReadPlayer();
}