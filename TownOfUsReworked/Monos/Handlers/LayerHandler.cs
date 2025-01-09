namespace TownOfUsReworked.Monos;

public class LayerHandler : RoleBehaviour
{
    public override bool IsDead => Player.HasDied();
    public override bool IsAffectedByComms => false;

    public bool Local => Player.AmOwner;

    [HideFromIl2Cpp]
    public Role CustomRole { get; set; }

    [HideFromIl2Cpp]
    public Ability CustomAbility { get; set; }

    [HideFromIl2Cpp]
    public Modifier CustomModifier { get; set; }

    [HideFromIl2Cpp]
    public Disposition CustomDisposition { get; set; }

    [HideFromIl2Cpp]
    public IEnumerable<PlayerLayer> CustomLayers { get; set; }

    [HideFromIl2Cpp]
    public IEnumerable<CustomButton> Buttons { get; set; }

    public static RoleBehaviour Crewmate;
    public static RoleBehaviour Impostor;
    public static RoleBehaviour CrewmateGhost;
    public static RoleBehaviour ImpostorGhost;

    public static Minigame HauntMenu;

    [HideFromIl2Cpp]
    public T GetLayer<T>() where T : PlayerLayer => CustomLayers.OfType<T>().FirstOrDefault();

    [HideFromIl2Cpp]
    public T GetILayer<T>() where T : IPlayerLayer => CustomLayers.OfType<T>().FirstOrDefault();

    public void UpdatePlayer()
    {
        CustomRole.UpdatePlayer();
        CustomAbility.UpdatePlayer();
        CustomModifier.UpdatePlayer();
        CustomDisposition.UpdatePlayer();
    }

    public void UpdatePlayer(PlayerControl __instance)
    {
        CustomRole.UpdatePlayer(__instance);
        CustomAbility.UpdatePlayer(__instance);
        CustomModifier.UpdatePlayer(__instance);
        CustomDisposition.UpdatePlayer(__instance);
    }

    public void UpdateVoteArea()
    {
        CustomRole.UpdateVoteArea();
        CustomAbility.UpdateVoteArea();
        CustomModifier.UpdateVoteArea();
        CustomDisposition.UpdateVoteArea();
    }

    public void UpdateVoteArea(PlayerVoteArea __instance)
    {
        CustomRole.UpdateVoteArea(__instance);
        CustomAbility.UpdateVoteArea(__instance);
        CustomModifier.UpdateVoteArea(__instance);
        CustomDisposition.UpdateVoteArea(__instance);
    }

    public void UpdateHud(HudManager __instance)
    {
        CustomRole.UpdateHud(__instance);
        CustomAbility.UpdateHud(__instance);
        CustomModifier.UpdateHud(__instance);
        CustomDisposition.UpdateHud(__instance);
        Buttons.ForEach(x => x.SetActive());
        CanVent = Player.CanVent();
    }

    public void UpdateMeeting(MeetingHud __instance)
    {
        CustomRole.UpdateMeeting(__instance);
        CustomAbility.UpdateMeeting(__instance);
        CustomModifier.UpdateMeeting(__instance);
        CustomDisposition.UpdateMeeting(__instance);
    }

    public void UponTaskComplete(uint idx)
    {
        CustomRole.UponTaskComplete(idx);
        CustomAbility.UponTaskComplete(idx);
        CustomModifier.UponTaskComplete(idx);
        CustomDisposition.UponTaskComplete(idx);

        if (AmongUsClient.Instance.AmHost)
            CheckEndGame.CheckEnd();

        if (!CustomRole.TasksDone)
            return;

        foreach (var button in Buttons)
        {
            if (button.HasUses)
            {
                button.uses++;
                button.maxUses++;
            }
        }
    }

    public void OnRevive()
    {
        CustomRole.OnRevive();
        CustomAbility.OnRevive();
        CustomModifier.OnRevive();
        CustomDisposition.OnRevive();
    }

    public void BeforeMeeting()
    {
        CustomRole.BeforeMeeting();
        CustomAbility.BeforeMeeting();
        CustomModifier.BeforeMeeting();
        CustomDisposition.BeforeMeeting();
    }

    public void OnIntroEnd()
    {
        CustomRole.OnIntroEnd();
        CustomAbility.OnIntroEnd();
        CustomModifier.OnIntroEnd();
        CustomDisposition.OnIntroEnd();
    }

    public void OnMeetingEnd(MeetingHud __instance)
    {
        CustomRole.OnMeetingEnd(__instance);
        CustomAbility.OnMeetingEnd(__instance);
        CustomModifier.OnMeetingEnd(__instance);
        CustomDisposition.OnMeetingEnd(__instance);
    }

    public void ResetButtons() => Buttons = Player.GetButtonsFromList();

    public void SetUpLayers()
    {
        CustomRole = Player.GetRoleFromList();
        CustomAbility = Player.GetAbilityFromList();
        CustomModifier = Player.GetModifierFromList();
        CustomDisposition = Player.GetDispositionFromList();

        if (!CustomRole)
        {
            CustomRole = new Roleless();
            CustomRole.Start(Player);
        }

        if (!CustomAbility)
        {
            CustomAbility = new Abilityless();
            CustomAbility.Start(Player);
        }

        if (!CustomModifier)
        {
            CustomModifier = new Modifierless();
            CustomModifier.Start(Player);
        }

        if (!CustomDisposition)
        {
            CustomDisposition = new Dispositionless();
            CustomDisposition.Start(Player);
        }

        CustomRole.PostAssignment();
        CustomAbility.PostAssignment();
        CustomModifier.PostAssignment();
        CustomDisposition.PostAssignment();

        CustomLayers = [ CustomRole, CustomModifier, CustomAbility, CustomDisposition ];
        ResetButtons();

        TasksCountTowardProgress = Player.CanDoTasks() && (CustomRole.Faction == Faction.Crew || CustomRole is Runner or Hunted);
        CanVent = Player.CanVent();
        CanUseKillButton = Player.CanKill();
        AffectedByLightAffectors = !(CustomAbility is Torch || CustomRole.Faction is Faction.Intruder or Faction.Syndicate);
    }

    public override float GetAbilityDistance() => GameSettings.InteractionDistance;

    public override void OnDeath(DeathReason reason)
    {
        if (CustomPlayer.LocalCustom.Dead)
            Flash(CustomColorManager.Stalemate);
        else if (CustomPlayer.Local.Is<Coroner>())
            Flash(CustomColorManager.Coroner);
        else if (CustomPlayer.Local.TryGetLayer<Monarch>(out var mon) && mon.Knighted.Contains(Player.PlayerId))
            Flash(mon.Color);

        TasksCountTowardProgress &= TaskSettings.GhostTasksCountToWin;
    }

    public override bool DidWin(GameOverReason gameOverReason) => CustomRole.Winner || CustomDisposition.Winner;

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl.AmOwner)
            PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl).Text = "Achieve your win condition!\n";
    }

    public override void AppendTaskHint(Il2CppSystem.Text.StringBuilder taskStringBuilder) {}

    public override void Initialize(PlayerControl player)
    {
        Player = player;

        SetUpLayers();

        IntroSound = GetAudio($"{CustomRole}Intro", false) ?? GetAudio($"{(CustomRole.Faction is Faction.Intruder or Faction.Syndicate ? "Impostor" : "Crewmate")}Intro");

        InitializeAbilityButton();

        if (player.AmOwner && !TutorialManager.InstanceExists && !TownOfUsReworked.MCIActive)
        {
            CustomStatsManager.IncrementStat(CustomRole.Faction switch
            {
                Faction.Crew => CustomStatsManager.StatsGamesCrew,
                Faction.Intruder => CustomStatsManager.StatsGamesIntruder,
                Faction.Neutral => CustomStatsManager.StatsGamesNeutral,
                Faction.Syndicate => CustomStatsManager.StatsGamesSyndicate,
                _ => StringNames.None
            });
        }
    }

    public override void OnMeetingStart()
    {
        CustomRole.OnMeetingStart(Meeting());
        CustomAbility.OnMeetingStart(Meeting());
        CustomModifier.OnMeetingStart(Meeting());
        CustomDisposition.OnMeetingStart(Meeting());
    }

    public override void OnVotingComplete()
    {
        CustomRole.VoteComplete(Meeting());
        CustomAbility.VoteComplete(Meeting());
        CustomModifier.VoteComplete(Meeting());
        CustomDisposition.VoteComplete(Meeting());
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        CustomRole.End();
        CustomAbility.End();
        CustomModifier.End();
        CustomDisposition.End();
    }

    public override bool CanUse(IUsable console)
    {
        // This is such a cheesy way to handle this omg
        var isCrew = CustomRole.Faction is Faction.Neutral or Faction.Crew || (CustomRole.Faction == Faction.GameMode && CustomRole.Type != LayerEnum.Hunter);
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
        else if (ActiveTask())
            return;
        else
        {
            var hud = HUD();
            var minigame = Instantiate(HauntMenu, hud.AbilityButton.transform, false);
            minigame.transform.SetLocalZ(-5f);
            minigame.Begin(null);
            hud.AbilityButton.SetDisabled();
        }
    }
}