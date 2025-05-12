namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Jester)]
public sealed class Jester : Evil
{
    [ToggleOption]
    public static bool JesterButton = true;

    [ToggleOption]
    private static bool JesterVent = false;

    [ToggleOption]
    private static bool JestSwitchVent = false;

    [ToggleOption]
    public static bool JestEjectScreen = false;

    public bool VotedOut { get; set; }
    private HashSet<byte> ToHaunt { get; } = [];
    private bool HasHaunted { get; set; }
    private CustomButton HauntButton { get; set; }
    public bool CanHaunt => VotedOut && !HasHaunted && ToHaunt.Any() && !NeutralSettings.AvoidNeutralKingmakers;

    protected override UColor MainColor => CustomColorManager.Jester;
    public override LayerEnum Type => LayerEnum.Jester;
    public override Func<string> StartText { get; } = () => "It Was Jest A Prank Bro";
    public override Func<string> Description => () => VotedOut ? "- You can haunt those who voted for you" : "- None";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override bool HasWon => VotedOut;
    public override bool CanVent => base.CanVent && JesterVent;
    public override bool CanSwitchVents => JestSwitchVent;
    public override WinLose EndState => WinLose.JesterWins;

    protected override void Init()
    {
        base.Init();
        Objectives = () => VotedOut ? "- You have been ejected" : "- Get ejected";
        ToHaunt.Clear();

        if (!NeutralSettings.AvoidNeutralKingmakers)
        {
            HauntButton ??= new(this, new SpriteName("Haunt"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Haunt, (PlayerBodyExclusion)Exception, new PostDeath(true),
                "HAUNT", (UsableFunc)Usable);
        }
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        if (VotedOut)
            return;

        ToHaunt.Clear();

        foreach (var state in __instance.playerStates)
        {
            var player = PlayerByVoteArea(state);

            if (state.AmDead || player.HasDied() || state.VotedFor != PlayerId || state.TargetPlayerId == PlayerId || Player.IsLinkedTo(player))
                continue;

            ToHaunt.Add(state.TargetPlayerId);
        }
    }

    private bool Exception(PlayerControl player) => !ToHaunt.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player) ||
        player.Is(Alignment.Deity);

    private void Haunt(PlayerControl target)
    {
        Player.RpcMurderPlayer(target, DeathReasonEnum.Haunted, false);
        HasHaunted = true;
    }

    private bool Usable() => CanHaunt;

    public override void OnKill(PlayerControl victim) => TrulyDead = true;
}