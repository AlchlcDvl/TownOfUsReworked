namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Jester : Neutral
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool JesterButton { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool JesterVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool JestSwitchVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool JestEjectScreen { get; set; } = false;

    public bool VotedOut { get; set; }
    public List<byte> ToHaunt { get; set; }
    public bool HasHaunted { get; set; }
    public CustomButton HauntButton { get; set; }
    public bool CanHaunt => VotedOut && !HasHaunted && ToHaunt.Any() && !NeutralSettings.AvoidNeutralKingmakers;

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Jester : CustomColorManager.Neutral;
    public override string Name => "Jester";
    public override LayerEnum Type => LayerEnum.Jester;
    public override Func<string> StartText => () => "It Was Jest A Prank Bro";
    public override Func<string> Description => () => VotedOut ? "- You can haunt those who voted for you" : "- None";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public override void Init()
    {
        BaseStart();
        Objectives = () => VotedOut ? "- You have been ejected" : "- Get ejected";
        Alignment = Alignment.NeutralEvil;
        ToHaunt = [];

        if (!NeutralSettings.AvoidNeutralKingmakers)
        {
            HauntButton ??= CreateButton(this, new SpriteName("Haunt"), AbilityType.Alive, KeybindType.ActionSecondary, (OnClick)Haunt, (PlayerBodyExclusion)Exception, new PostDeath(true),
                "HAUNT", (UsableFunc)Usable);
        }
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        base.VoteComplete(__instance);

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

    public bool Exception(PlayerControl player) => !ToHaunt.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player) ||
        player.Is(Alignment.NeutralApoc);

    public void Haunt()
    {
        RpcMurderPlayer(Player, HauntButton.GetTarget<PlayerControl>(), DeathReasonEnum.Haunted, false);
        HasHaunted = true;
        TrulyDead = true;
    }

    public bool Usable() => CanHaunt;
}