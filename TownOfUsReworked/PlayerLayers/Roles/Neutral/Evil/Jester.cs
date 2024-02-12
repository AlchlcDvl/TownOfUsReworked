namespace TownOfUsReworked.PlayerLayers.Roles;

public class Jester : Neutral
{
    public bool VotedOut { get; set; }
    public List<byte> ToHaunt { get; set; }
    public bool HasHaunted { get; set; }
    public CustomButton HauntButton { get; set; }
    public bool CanHaunt => VotedOut && !HasHaunted && ToHaunt.Count > 0 && !CustomGameOptions.AvoidNeutralKingmakers;

    public override UColor Color => ClientGameOptions.CustomNeutColors ? CustomColorManager.Jester : CustomColorManager.Neutral;
    public override string Name => "Jester";
    public override LayerEnum Type => LayerEnum.Jester;
    public override Func<string> StartText => () => "It Was Jest A Prank Bro";
    public override Func<string> Description => () => VotedOut ? "- You can haunt those who voted for you" : "- None";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public Jester() : base() {}

    public override PlayerLayer Start(PlayerControl player)
    {
        SetPlayer(player);
        BaseStart();
        Objectives = () => VotedOut ? "- You have been ejected" : "- Get ejected";
        Alignment = Alignment.NeutralEvil;
        ToHaunt = new();
        HauntButton = new(this, "Haunt", AbilityTypes.Alive, "ActionSecondary", Haunt, Exception, true);
        return this;
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
        RpcMurderPlayer(Player, HauntButton.TargetPlayer, DeathReasonEnum.Haunted, false);
        HasHaunted = true;
        TrulyDead = true;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        HauntButton.Update2("HAUNT", CanHaunt);
    }
}