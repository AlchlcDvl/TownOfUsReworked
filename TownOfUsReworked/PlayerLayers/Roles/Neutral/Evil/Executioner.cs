namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Executioner : Evil
{
    [ToggleOption]
    public static bool ExecutionerCanPickTargets = false;

    [ToggleOption]
    public static bool ExecutionerButton = true;

    [ToggleOption]
    public static bool ExeVent = false;

    [ToggleOption]
    public static bool ExeSwitchVent = false;

    [ToggleOption]
    public static bool ExeTargetKnows = false;

    [ToggleOption]
    public static bool ExeKnowsTargetRole = false;

    [ToggleOption]
    public static bool ExeEjectScreen = false;

    [ToggleOption]
    public static bool ExeCanWinBeyondDeath = false;

    [ToggleOption]
    public static bool ExeToJest = true;

    public PlayerControl TargetPlayer { get; set; }
    public bool TargetVotedOut { get; set; }
    public List<byte> ToDoom { get; } = [];
    public bool HasDoomed { get; set; }
    public CustomButton DoomButton { get; set; }
    public bool CanDoom => TargetPlayer && TargetVotedOut && !HasDoomed && ToDoom.Any() && !NeutralSettings.AvoidNeutralKingmakers;
    public bool Failed => !TargetVotedOut && TargetPlayer.HasDied();
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool TargetFailed => !TargetPlayer && Rounds > 2;

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Executioner : FactionColor;
    public override LayerEnum Type => LayerEnum.Executioner;
    public override Func<string> StartText => () => "Find Someone To Eject";
    public override Func<string> Description => () => TargetPlayer ? ((TargetVotedOut ? $"- You can doom those who voted for {TargetPlayer?.name}\n" : "") +
        $"- If {TargetPlayer?.name} dies, you will become a <#F7B3DAFF>Jester</color>") : "- You can select a player to eject";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;
    public override bool HasWon => TargetVotedOut;
    public override WinLose EndState => WinLose.ExecutionerWins;

    public override void Init()
    {
        base.Init();
        Objectives = () => TargetVotedOut ? $"- {TargetPlayer?.name} has been ejected" : (!TargetPlayer ? "- Find a target to eject" : $"- Eject {TargetPlayer?.name}");
        ToDoom.Clear();

        if (!NeutralSettings.AvoidNeutralKingmakers)
            DoomButton ??= new(this, new SpriteName("Doom"), AbilityTypes.Player, KeybindType.ActionSecondary, (OnClickPlayer)Doom, (PlayerBodyExclusion)Exception1, "DOOM", (UsableFunc)Usable1);

        Rounds = 0;
    }

    public override void PostAssignment()
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

    public void SelectTarget(PlayerControl target)
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

    public void TurnJest() => new Jester().RoleUpdate(this);

    public void Doom(PlayerControl target)
    {
        Player.RpcMurderPlayer(target, DeathReasonEnum.Doomed, false);
        HasDoomed = true;
    }

    public bool Exception1(PlayerControl player) => !ToDoom.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || player.IsLinkedTo(Player) ||
        player.Is(Alignment.Apocalypse);

    public bool Exception2(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.Is(Alignment.Sovereign) || (player.Is(SubFaction) && SubFaction !=
        SubFaction.None);

    public bool Usable1() => CanDoom;

    public bool Usable2() => !TargetPlayer;

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);

        if ((TargetFailed || (TargetPlayer && Failed)) && !Dead)
        {
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
}