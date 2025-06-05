namespace TownOfUsReworked.PlayerLayers.Roles;

[LayerHeaderOption(LayerEnum.Executioner)]
public sealed class Executioner : Evil
{
    [ToggleOption]
    public static bool ExecutionerCanPickTargets = false;

    [ToggleOption]
    public static bool ExecutionerButton = true;

    [ToggleOption]
    private static bool ExeVent = false;

    [ToggleOption]
    private static bool ExeSwitchVent = false;

    [ToggleOption]
    public static bool ExeTargetKnows = false;

    [ToggleOption]
    private static bool ExeKnowsTargetRole = false;

    [ToggleOption]
    public static bool ExeEjectScreen = false;

    [ToggleOption]
    public static bool ExeCanWinBeyondDeath = false;

    [ToggleOption]
    private static bool ExeToJest = true;

    public PlayerControl TargetPlayer { get; set; }
    public bool TargetVotedOut { get; set; }
    private HashSet<byte> ToDoom { get; } = [];
    private bool HasDoomed { get; set; }
    private CustomButton DoomButton { get; set; }
    private bool CanDoom => TargetPlayer && TargetVotedOut && !HasDoomed && ToDoom.Any() && !OutcastSettings.AvoidOutcastKingmakers;
    private bool Failed => !TargetVotedOut && TargetPlayer.HasDied();
    private int Rounds { get; set; }
    private CustomButton TargetButton { get; set; }
    private bool TargetFailed => !TargetPlayer && Rounds > 2;

    protected override UColor MainColor => CustomColorManager.Executioner;
    public override LayerEnum Type => LayerEnum.Executioner;
    public override Func<string> StartText { get; } = () => "Find Someone To Eject";
    public override Func<string> Description => () => TargetPlayer ? ((TargetVotedOut ? $"- You can doom those who voted for {TargetPlayer?.name}\n" : "") +
        $"- If {TargetPlayer?.name} dies, you will become a <#F7B3DAFF>Jester</color>") : "- You can select a player to eject";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override bool HasWon => TargetVotedOut;
    public override bool CanVent => base.CanVent && ExeVent;
    public override bool CanSwitchVents => ExeSwitchVent;
    public override WinLose EndState => WinLose.ExecutionerWins;

    protected override void Init()
    {
        base.Init();
        Objectives = () => TargetVotedOut ? $"- {TargetPlayer?.name} has been ejected" : (!TargetPlayer ? "- Find a target to eject" : $"- Eject {TargetPlayer?.name}");
        ToDoom.Clear();

        if (!OutcastSettings.AvoidOutcastKingmakers)
            DoomButton ??= new(this, new SpriteName("Doom"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Doom, (PlayerBodyExclusion)Exception1, "DOOM", (UsableFunc)Usable1);

        Rounds = 0;
    }

    public override void Reset(bool meeting, bool start)
    {
        if (meeting && !TargetPlayer)
            Rounds++;
    }

    public override void UpdatePlayerName(LayerHandler handler, PlayerControl player, bool meeting, ref string name, ref UColor color, ref bool revealed, ref bool removeFromConsig)
    {
        if (player != TargetPlayer)
            return;

        name += " <#CCCCCCFF>§</color>";

        if (!ExeKnowsTargetRole || revealed)
            return;

        var role = handler.CurrentRole;
        color = role.Color;
        name += $"\n{role}";
        revealed = true;
    }

    public override void LateInit()
    {
        if (ExecutionerCanPickTargets || !TargetPlayer)
        {
            TargetButton ??= new(this, new SpriteName("ExeTarget"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)SelectTarget, (PlayerBodyExclusion)Exception2, "TORMENT",
                (UsableFunc)Usable2);
        }
    }

    public override List<PlayerControl> Team()
    {
        var team = base.Team();
        team.Add(TargetPlayer);
        return team;
    }

    private void SelectTarget(PlayerControl target)
    {
        TargetPlayer = target;
        CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, TargetPlayer);
    }

    public override void VoteComplete(MeetingHud __instance)
    {
        if (TargetVotedOut)
            return;

        ToDoom.Clear();

        foreach (var state in __instance.playerStates)
        {
            var player = PlayerByVoteArea(state);

            if (state.AmDead || player.HasDied() || state.VotedFor != TargetPlayer.PlayerId || state.TargetPlayerId == PlayerId || Player.IsLinkedTo(player))
                continue;

            ToDoom.Add(state.TargetPlayerId);
        }
    }

    private void TurnJest() => new Jester().RoleUpdate(this);

    private void Doom(PlayerControl target)
    {
        Player.RpcMurderPlayer(target, DeathReasonEnum.Doomed, false);
        HasDoomed = true;
    }

    private bool Exception1(PlayerControl player) => !ToDoom.Contains(player.PlayerId) || player.IsLinkedTo(Player) || player.Is(Alignment.Deity);

    private bool Exception2(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.Is(Alignment.Sovereign);

    private bool Usable1() => CanDoom;

    private bool Usable2() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        if ((!TargetFailed && (!TargetPlayer || !Failed)) || Dead)
            return;

        if (ExeToJest)
        {
            CallRpc(CustomRPC.Misc, MiscRPC.ChangeRoles, this);
            TurnJest();
        }
        else if (ExecutionerCanPickTargets)
        {
            TargetPlayer = null;
            Rounds = 0;
            CallRpc(CustomRPC.Misc, MiscRPC.SetTarget, this, 255);
        }
        else
            Player.RpcSuicide();
    }
}