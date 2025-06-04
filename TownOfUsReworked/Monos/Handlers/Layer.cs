namespace TownOfUsReworked.Monos;

public sealed class LayerHandler : RoleBehaviour
{
    public override bool IsDead => Player.HasDied();
    public override bool IsAffectedByComms => false;

    public Faction CurrentFaction;

    public readonly List<Faction> FakeFactions = [];
    public readonly List<(LayerEnum, Faction)> History = [];

    private bool Local => Player.AmOwner;

    /// <summary>
    /// Gets or sets a value indicating whether or not the layer is a winner.
    /// </summary>
    public bool Winner { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the player has disconnected.
    /// </summary>
    public bool Disconnected => Player?.Data?.Disconnected ?? true;

    [HideFromIl2Cpp]
    public Role CurrentRole { get; set; }

    [HideFromIl2Cpp]
    public Ability CurrentAbility { get; set; }

    [HideFromIl2Cpp]
    public Modifier CurrentModifier { get; set; }

    [HideFromIl2Cpp]
    public Disposition CurrentDisposition { get; set; }

    [HideFromIl2Cpp]
    public IEnumerable<PlayerLayer> CurrentLayers { get; set; }

    [HideFromIl2Cpp]
    public IEnumerable<CustomButton> Buttons { get; set; }

    public static RoleBehaviour Crewmate;
    public static RoleBehaviour Impostor;
    public static RoleBehaviour CrewmateGhost;
    public static RoleBehaviour ImpostorGhost;

    public static Minigame HauntMenu;

    [HideFromIl2Cpp]
    public T GetLayer<T>() where T : IPlayerLayer => CurrentLayers.OfType<T>().FirstOrDefault();

    public void UpdatePlayer()
    {
        CurrentRole.UpdatePlayer();
        CurrentAbility.UpdatePlayer();
        CurrentModifier.UpdatePlayer();
        CurrentDisposition.UpdatePlayer();
    }

    public void UpdatePlayer(PlayerControl __instance)
    {
        CurrentRole.UpdatePlayer(__instance);
        CurrentAbility.UpdatePlayer(__instance);
        CurrentModifier.UpdatePlayer(__instance);
        CurrentDisposition.UpdatePlayer(__instance);
    }

    public void UpdateVoteArea()
    {
        CurrentRole.UpdateVoteArea();
        CurrentAbility.UpdateVoteArea();
        CurrentModifier.UpdateVoteArea();
        CurrentDisposition.UpdateVoteArea();
    }

    public void UpdateVoteArea(PlayerVoteArea __instance)
    {
        CurrentRole.UpdateVoteArea(__instance);
        CurrentAbility.UpdateVoteArea(__instance);
        CurrentModifier.UpdateVoteArea(__instance);
        CurrentDisposition.UpdateVoteArea(__instance);
    }

    public void UpdateHud(HudManager __instance)
    {
        CurrentRole.UpdateHud(__instance);
        CurrentAbility.UpdateHud(__instance);
        CurrentModifier.UpdateHud(__instance);
        CurrentDisposition.UpdateHud(__instance);
        Buttons.Do(x => x.SetActive());
        CanVent = Player.CanVent();
    }

    public void UpdateMeeting(MeetingHud __instance)
    {
        CurrentRole.UpdateMeeting(__instance);
        CurrentAbility.UpdateMeeting(__instance);
        CurrentModifier.UpdateMeeting(__instance);
        CurrentDisposition.UpdateMeeting(__instance);
    }

    public void UponTaskComplete(uint idx)
    {
        CurrentRole.UponTaskComplete(idx);
        CurrentAbility.UponTaskComplete(idx);
        CurrentModifier.UponTaskComplete(idx);
        CurrentDisposition.UponTaskComplete(idx);

        if (AmongUsClient.Instance.AmHost && CurrentRole is Runner { TasksDone: true })
        {
            WinState = WinLose.TaskRunnerWins;
            Winner = true;
            CallRpc(CustomRPC.Misc, MiscRPC.WinLose, WinState, Player);
        }

        if (!CurrentRole.TasksDone)
            return;

        foreach (var button in Buttons.Where(x => x.HasUses))
        {
            button.UsesCount++;
            button.Max++;
        }
    }

    public void OnRevive()
    {
        CurrentRole.OnRevive();
        CurrentAbility.OnRevive();
        CurrentModifier.OnRevive();
        CurrentDisposition.OnRevive();
    }

    public void BeforeMeeting()
    {
        CurrentRole.BeforeMeeting();
        CurrentAbility.BeforeMeeting();
        CurrentModifier.BeforeMeeting();
        CurrentDisposition.BeforeMeeting();
    }

    public void OnIntroEnd()
    {
        CurrentRole.OnIntroEnd();
        CurrentAbility.OnIntroEnd();
        CurrentModifier.OnIntroEnd();
        CurrentDisposition.OnIntroEnd();
    }

    public void OnMeetingEnd(MeetingHud __instance)
    {
        CurrentRole.OnMeetingEnd(__instance);
        CurrentAbility.OnMeetingEnd(__instance);
        CurrentModifier.OnMeetingEnd(__instance);
        CurrentDisposition.OnMeetingEnd(__instance);
    }

    public void ResetButtons() => Buttons = Player.GetButtonsFromList();

    [HideFromIl2Cpp]
    public void SetUpLayers()
    {
        CurrentRole = Player.GetRoleFromList();
        CurrentAbility = Player.GetAbilityFromList();
        CurrentModifier = Player.GetModifierFromList();
        CurrentDisposition = Player.GetDispositionFromList();

        if (!CurrentRole)
        {
            CurrentRole = new Roleless();
            CurrentRole.Start(Player);
        }

        if (!CurrentAbility)
        {
            CurrentAbility = new Abilityless();
            CurrentAbility.Start(Player);
        }

        if (!CurrentModifier)
        {
            CurrentModifier = new Modifierless();
            CurrentModifier.Start(Player);
        }

        if (!CurrentDisposition)
        {
            CurrentDisposition = new Dispositionless();
            CurrentDisposition.Start(Player);
        }

        CurrentRole.PostAssignment();
        CurrentAbility.PostAssignment();
        CurrentModifier.PostAssignment();
        CurrentDisposition.PostAssignment();

        CurrentLayers = [ CurrentRole, CurrentModifier, CurrentAbility, CurrentDisposition ];
        ResetButtons();

        TasksCountTowardProgress = Player.CanDoTasks() && (CurrentRole is Runner or Hunted || CurrentFaction == Faction.Crew);
        CanVent = Player.CanVent();
        AffectedByLightAffectors = !(CurrentAbility is Torch || !CurrentRole.AffectedByLights);

        CurrentLayers.Do([HideFromIl2Cpp] (x) => x.Handler = this);

        Player.GetComponent<PlayerControlHandler>().UpdateCurrent();

        History.Add((CurrentRole.Type, CurrentRole.Faction));
    }

    public override float GetAbilityDistance() => GameOptions.InteractionDistance;

    public override void OnDeath(DeathReason reason)
    {
        if (LocalPlayer.HasDied())
            Flash(CustomColorManager.Stalemate);
        else if (LocalPlayer.Is<Coroner>())
            Flash(CustomColorManager.Coroner);
        else if (LocalPlayer.Is<Monarch>(out var mon) && mon.Knighted.Contains(Player.PlayerId))
            Flash(mon.Color);

        TasksCountTowardProgress &= TaskOptions.GhostTasksCountToWin;
    }

    public override bool DidWin(GameOverReason gameOverReason) => Winner;

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl.AmOwner)
            PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl).Text = "Achieve your win condition!\n";
    }

    public override void AppendTaskHint(IStringBuilder taskStringBuilder) {}

    public override void Initialize(PlayerControl player)
    {
        Player = player;

        SetUpLayers();

        IntroSound = GetAudio($"{CurrentRole}Intro", false) ?? GetAudio($"{(CurrentRole is Intruder or Syndicate ? "Impostor" : "Crewmate")}Intro");

        InitializeAbilityButton();

        if (player.AmOwner && !TutorialManager.InstanceExists && !TownOfUsReworked.MciActive)
        {
            CustomStatsManager.IncrementStat(CurrentRole.Faction switch
            {
                Faction.Crew => CustomStatsManager.StatsGamesCrew,
                Faction.Intruder => CustomStatsManager.StatsGamesIntruder,
                Faction.Outcast => CustomStatsManager.StatsGamesOutcast,
                Faction.Syndicate => CustomStatsManager.StatsGamesSyndicate,
                Faction.Apocalypse => CustomStatsManager.StatsGamesApocalypse,
                Faction.Pandorica => CustomStatsManager.StatsGamesPandorica,
                Faction.Compliance => CustomStatsManager.StatsGamesCompliance,
                Faction.Illuminati => CustomStatsManager.StatsGamesIlluminati,
                _ => StringNames.None
            });
        }
    }

    public override void OnMeetingStart()
    {
        if (!Local)
            return;

        CurrentRole.OnMeetingStart(Meeting());
        CurrentAbility.OnMeetingStart(Meeting());
        CurrentModifier.OnMeetingStart(Meeting());
        CurrentDisposition.OnMeetingStart(Meeting());
    }

    public override void OnVotingComplete()
    {
        if (!Local)
            return;

        CurrentRole.VoteComplete(Meeting());
        CurrentAbility.VoteComplete(Meeting());
        CurrentModifier.VoteComplete(Meeting());
        CurrentDisposition.VoteComplete(Meeting());
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        CurrentRole.End();
        CurrentAbility.End();
        CurrentModifier.End();
        CurrentDisposition.End();
    }

    public override bool CanUse(IUsable console)
    {
        // This is such a cheesy way to handle this omg
        var isCrew = CurrentRole.Faction is Faction.Outcast or Faction.Crew || (CurrentRole.Faction == Faction.GameMode && CurrentRole.Type != LayerEnum.Hunter);
        var role = IsDead ? (isCrew ? CrewmateGhost : ImpostorGhost) : (isCrew ? Crewmate : Impostor);
        role.Player = Player;
        var result = role.CanUse(console);
        role.Player = null;
        return result;
    }

    public override void UseAbility()
    {
        if (Chat().IsOpenOrOpening)
            return;

        if (ActiveTask() is HauntMenuMinigame)
            ActiveTask().Close();
        else if (!ActiveTask())
        {
            var hud = HUD();
            var minigame = Instantiate(HauntMenu, hud.AbilityButton.transform, false);
            minigame.transform.SetLocalZ(-5f);
            minigame.Begin(null);
            hud.AbilityButton.SetDisabled();
        }
    }
}