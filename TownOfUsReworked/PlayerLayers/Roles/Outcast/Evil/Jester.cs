namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(Layer.Jester)]
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

    public bool VotedOut;
    private readonly HashSet<byte> ToHaunt = [];
    private bool HasHaunted;
    private CustomButton HauntButton;
    public bool CanHaunt => VotedOut && !HasHaunted && ToHaunt.Any() && !OutcastSettings.AvoidOutcastKingmakers;

    protected override UColor MainColor => CustomColorManager.Jester;
    public override Layer Type => Layer.Jester;
    public override string StartText => "It Was Jest A Prank Bro";
    public override string Description => VotedOut ? "- You can haunt those who voted for you" : "- None";
    public override Attack Attack => Attack.Unstoppable;
    public override bool HasWon => VotedOut;
    public override bool CanVent => base.CanVent && JesterVent;
    public override bool CanSwitchVents => JestSwitchVent;
    protected override WinLose EndState => WinLose.JesterWins;

    public override void Init()
    {
        base.Init();
        Objectives = () => VotedOut ? "- You have been ejected" : "- Get ejected";
        ToHaunt.Clear();

        if (!OutcastSettings.AvoidOutcastKingmakers)
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

    private bool Exception(PlayerControl player) => !ToHaunt.Contains(player.PlayerId) || Player.IsLinkedTo(player) ||
        player.Is(Alignment.Deity);

    private void Haunt(PlayerControl target)
    {
        Player.RpcMurderPlayer(target, DeathReasonEnum.Haunted, false);
        HasHaunted = true;
    }

    private bool Usable() => CanHaunt;

    public override void OnKill(PlayerControl victim) => TrulyDead = true;
}