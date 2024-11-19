namespace TownOfUsReworked.PlayerLayers.Roles;

[HeaderOption(MultiMenu.LayerSubOptions)]
public class Executioner : Neutral
{
    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExecutionerCanPickTargets { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExecutionerButton { get; set; } = true;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExeVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExeSwitchVent { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExeTargetKnows { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExeKnowsTargetRole { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExeEjectScreen { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExeCanWinBeyondDeath { get; set; } = false;

    [ToggleOption(MultiMenu.LayerSubOptions)]
    public static bool ExeToJest { get; set; } = true;

    public PlayerControl TargetPlayer { get; set; }
    public bool TargetVotedOut { get; set; }
    public List<byte> ToDoom { get; set; }
    public bool HasDoomed { get; set; }
    public CustomButton DoomButton { get; set; }
    public bool CanDoom => TargetPlayer && TargetVotedOut && !HasDoomed && ToDoom.Any() && !NeutralSettings.AvoidNeutralKingmakers;
    public bool Failed => !TargetVotedOut && TargetPlayer.HasDied();
    public int Rounds { get; set; }
    public CustomButton TargetButton { get; set; }
    public bool TargetFailed => !TargetPlayer && Rounds > 2;

    public override UColor Color => ClientOptions.CustomNeutColors ? CustomColorManager.Executioner : CustomColorManager.Neutral;
    public override string Name => "Executioner";
    public override LayerEnum Type => LayerEnum.Executioner;
    public override Func<string> StartText => () => "Find Someone To Eject";
    public override Func<string> Description => () => TargetPlayer ? ((TargetVotedOut ? $"- You can doom those who voted for {TargetPlayer?.name}\n" : "") +
        $"- If {TargetPlayer?.name} dies, you will become a <color=#F7B3DAFF>Jester</color>") : "- You can select a player to eject";
    public override AttackEnum AttackVal => AttackEnum.Unstoppable;

    public override void Init()
    {
        base.Init();
        Objectives = () => TargetVotedOut ? $"- {TargetPlayer?.name} has been ejected" : (!TargetPlayer ? "- Find a target to eject" : $"- Eject {TargetPlayer?.name}");
        Alignment = Alignment.NeutralEvil;
        ToDoom = [];

        if (ExecutionerCanPickTargets)
        {
            TargetButton ??= new(this, new SpriteName("ExeTarget"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)SelectTarget, (PlayerBodyExclusion)Exception2, "TORMENT",
                (UsableFunc)Usable2);
        }

        if (!NeutralSettings.AvoidNeutralKingmakers)
        {
            DoomButton ??= new(this, new SpriteName("Doom"), AbilityTypes.Alive, KeybindType.ActionSecondary, (OnClick)Doom, (PlayerBodyExclusion)Exception1, "DOOM",
                (UsableFunc)Usable1);
        }

        Rounds = 0;
    }

    public void SelectTarget()
    {
        TargetPlayer = TargetButton.GetTarget<PlayerControl>();
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

    public void TurnJest() => new Jester().RoleUpdate(this, Player);

    public void Doom()
    {
        RpcMurderPlayer(Player, DoomButton.GetTarget<PlayerControl>(), DeathReasonEnum.Doomed, false);
        HasDoomed = true;
    }

    public bool Exception1(PlayerControl player) => !ToDoom.Contains(player.PlayerId) || (player.Is(SubFaction) && SubFaction != SubFaction.None) || player.IsLinkedTo(Player) ||
        player.Is(Alignment.NeutralApoc);

    public bool Exception2(PlayerControl player) => player == TargetPlayer || player.IsLinkedTo(Player) || player.Is(Alignment.CrewSov) || (player.Is(SubFaction) && SubFaction !=
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
                RpcMurderPlayer(Player);
        }
    }
}