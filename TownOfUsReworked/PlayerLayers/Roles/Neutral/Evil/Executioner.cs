namespace TownOfUsReworked.PlayerLayers.Roles;

public class Executioner : Neutral
{
    public PlayerControl TargetPlayer { get; set; }
    public bool TargetVotedOut { get; set; }
    public List<byte> ToDoom { get; set; }
    public bool HasDoomed { get; set; }
    public CustomButton DoomButton { get; set; }
    public bool CanDoom => TargetPlayer != null && TargetVotedOut && !HasDoomed && ToDoom.Count > 0 && !CustomGameOptions.AvoidNeutralKingmakers;
    public bool Failed => !TargetVotedOut && TargetPlayer.HasDied();
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool TargetFailed => TargetPlayer == null && Rounds > 2;

    public override Color Color => ClientGameOptions.CustomNeutColors ? Colors.Executioner : Colors.Neutral;
    public override string Name => "Executioner";
    public override LayerEnum Type => LayerEnum.Executioner;
    public override Func<string> StartText => () => "Find Someone To Eject";
    public override Func<string> Description => () => TargetPlayer == null ? "- You can select a player to eject" : ((TargetVotedOut ? "- You can doom those who voted for " +
        $"{TargetPlayer?.name}\n" : "") + $"- If {TargetPlayer?.name} dies, you will become a <color=#F7B3DAFF>Jester</color>");
    public override InspectorResults InspectorResults => InspectorResults.Manipulative;

    public Executioner(PlayerControl player) : base(player)
    {
        Objectives = () => TargetVotedOut ? $"- {TargetPlayer?.name} has been ejected" : (TargetPlayer == null ? "- Find a target to eject" : $"- Eject {TargetPlayer?.name}");
        Alignment = Alignment.NeutralEvil;
        ToDoom = new();
        DoomButton = new(this, "Doom", AbilityTypes.Target, "ActionSecondary", Doom, Exception1);
        TargetButton = new(this, "ExeTarget", AbilityTypes.Target, "ActionSecondary", SelectTarget, Exception2);
        Rounds = 0;
    }

    public void SelectTarget()
    {
        TargetPlayer = TargetButton.TargetPlayer;
        CallRpc(CustomRPC.Target, TargetRPC.SetExeTarget, this, TargetPlayer);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        base.VoteComplete(__instance);

        if (TargetVotedOut)
            return;

        ToDoom.Clear();

        foreach (var state in __instance.playerStates)
        {
            var player = PlayerByVoteArea(state);

            if (state.AmDead || player.Data.Disconnected || state.VotedFor != TargetPlayer.PlayerId || state.TargetPlayerId == PlayerId || Player.IsLinkedTo(player))
                continue;

            ToDoom.Add(state.TargetPlayerId);
        }
    }

    public void TurnJest()
    {
        var newRole = new Jester(Player);
        newRole.RoleUpdate(this);
    }

    public void Doom()
    {
        RpcMurderPlayer(Player, DoomButton.TargetPlayer, DeathReasonEnum.Doomed, false);
        HasDoomed = true;
    }

    public bool Exception1(PlayerControl player) => !ToDoom.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || player.IsLinkedTo(Player);

    public bool Exception2(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.Is(Alignment.CrewSov) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None);

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        DoomButton.Update2("DOOM", CanDoom);
        TargetButton.Update2("TORMENT", TargetPlayer == null);

        if ((TargetFailed || (TargetPlayer != null && Failed)) && !IsDead)
        {
            CallRpc(CustomRPC.Change, TurnRPC.TurnJest, this);
            TurnJest();
        }
    }
}