namespace TownOfUsReworked.PlayerLayers.Roles;

public class Jester : Neutral
{
    public bool VotedOut { get; set; }
    public List<byte> ToHaunt { get; set; }
    public bool HasHaunted { get; set; }
    public CustomButton HauntButton { get; set; }
    public bool CanHaunt => VotedOut && !HasHaunted && ToHaunt.Count > 0 && !CustomGameOptions.AvoidNeutralKingmakers;

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Jester : Colors.Neutral;
    public override string Name => "Jester";
    public override LayerEnum Type => LayerEnum.Jester;
    public override Func<string> StartText => () => "It Was Jest A Prank Bro";
    public override Func<string> Description => () => VotedOut ? "- You can haunt those who voted for you" : "- None";

    public Jester(PlayerControl player) : base(player)
    {
        Objectives = () => VotedOut ? "- You have been ejected" : "- Get ejected";
        Alignment = Alignment.NeutralEvil;
        ToHaunt = new();
        HauntButton = new(this, "Haunt", AbilityTypes.Target, "ActionSecondary", Haunt, Exception, true);
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

            if (state.AmDead || player.Data.Disconnected || state.VotedFor != PlayerId || state.TargetPlayerId == PlayerId || Player.IsLinkedTo(player))
                continue;

            ToHaunt.Add(state.TargetPlayerId);
        }
    }

    public bool Exception(PlayerControl player) => !ToHaunt.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || Player.IsLinkedTo(player);

    public void Haunt()
    {
        RpcMurderPlayer(Player, HauntButton.TargetPlayer, DeathReasonEnum.Haunted, false);
        HasHaunted = true;
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        HauntButton.Update2("HAUNT", CanHaunt);
    }
}