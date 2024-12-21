namespace TownOfUsReworked.Monos;

public class LayerHandler : RoleBehaviour
{
    public override bool IsDead => Player?.Data?.IsDead ?? false;
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

        foreach (var button in Buttons)
        {
            if (button.HasUses)
            {
                button.Uses++;
                button.MaxUses++;
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

    [HideFromIl2Cpp]
    public void SetUpLayers(Role newRole = null)
    {
        CustomRole = newRole ?? Player.GetRole();
        CustomAbility = Player.GetAbility();
        CustomModifier = Player.GetModifier();
        CustomDisposition = Player.GetDisposition();
        CustomLayers = [ CustomRole, CustomModifier, CustomAbility, CustomDisposition ];
        ResetButtons();
    }

    public override float GetAbilityDistance() => GameSettings.InteractionDistance;

    public override void OnDeath(DeathReason reason)
    {
        if (CustomPlayer.LocalCustom.Dead)
            Flash(CustomColorManager.Stalemate);
        else if (CustomPlayer.Local.Is<Coroner>())
            Flash(CustomColorManager.Coroner);
        else if (CustomPlayer.Local.TryGetLayer<Monarch>(out var mon) && mon.Knighted.Contains(Player.PlayerId))
            Flash(CustomColorManager.Monarch);
    }

    public override bool DidWin(GameOverReason gameOverReason)
    {
        if (gameOverReason != (GameOverReason)9)
            return false;

        return CustomRole.Winner || CustomDisposition.Winner;
    }

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

        CustomRole.PostAssignment();
        CustomAbility.PostAssignment();
        CustomModifier.PostAssignment();
        CustomDisposition.PostAssignment();
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
        CustomRole.Deinit();
        CustomAbility.Deinit();
        CustomModifier.Deinit();
        CustomDisposition.Deinit();
    }

    public override bool CanUse(IUsable console)
    {
        // This is such a cheesy way to handle this omg
        var isCrew = CustomRole.Faction is Faction.Neutral or Faction.Crew || (CustomRole.Faction == Faction.GameMode && CustomRole.Type != LayerEnum.Hunter);
        var role = Player.HasDied() ? (isCrew ? CrewmateGhost : ImpostorGhost) : (isCrew ? Crewmate : Impostor);
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
            var minigame = Instantiate(HauntMenu, HUD().AbilityButton.transform, false);
            minigame.transform.SetLocalZ(-5f);
            minigame.Begin(null);
            HUD().AbilityButton.SetDisabled();
        }
    }
}