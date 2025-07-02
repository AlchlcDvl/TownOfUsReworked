namespace TownOfUsReworked.Monos;

public sealed class LayerHandler : RoleBehaviour
{
    public static readonly Dictionary<byte, LayerHandler> Handlers = [];
    public static RoleTypes Type;

    public override bool IsDead => Player.HasDied();
    public override bool IsAffectedByComms => false;

    public Faction CurrentFaction
    {
        get;
        set
        {
            if (field == value)
                return;

            CurrentRole.FactionColor = value switch
            {
                Faction.Intruder => CustomColorManager.Intruder,
                Faction.Crew => CustomColorManager.Crew,
                Faction.Syndicate => CustomColorManager.Syndicate,
                Faction.Pandorica => CustomColorManager.Pandorica,
                Faction.Compliance => CustomColorManager.Compliance,
                Faction.Illuminati => CustomColorManager.Illuminati,
                Faction.Apocalypse => CustomColorManager.Apocalypse,
                Faction.Undead => CustomColorManager.Undead,
                Faction.Cult => CustomColorManager.Cult,
                Faction.Cabal => CustomColorManager.Cabal,
                Faction.Reanimated => CustomColorManager.Reanimated,
                Faction.Followers => CustomColorManager.Followers,
                Faction.GameMode => CurrentRole.Alignment switch
                {
                    Alignment.HideAndSeek => CustomColorManager.HideAndSeek,
                    Alignment.TaskRace => CustomColorManager.TaskRace,
                    _ => CustomColorManager.Faction
                },
                _ when value.IsOutcast() => CustomColorManager.Outcast,
                _ => CustomColorManager.Faction
            };
            CurrentRole.Objectives = value switch
            {
                Faction.Intruder => () => IntrudersWinCon(Player),
                Faction.Syndicate => () => SyndicateWinCon(Player),
                Faction.Apocalypse => () => ApocalypseWinCon(Player),
                Faction.Compliance => () => ComplianceWinCon(Player),
                Faction.Pandorica => () => PandoricaWinCon(Player),
                Faction.Illuminati => () => IlluminatiWinCon(Player),
                Faction.Crew => () => CrewWinCon,
                Faction.Undead => UndeadWinCon,
                Faction.Cabal => CabalWinCon,
                Faction.Cult => CultWinCon,
                Faction.Followers => FollowersWinCon,
                Faction.Reanimated => ReanimatedWinCon,
                _ => CurrentRole.Objectives
            };

            if (Local)
                UpdateButtons();

            if (field != Faction.None)
            {
                History.Add((CurrentRole.Type, field));
                FakeFactions.Add(field);
            }

            field = value;

            if (CurrentAbility is Assassin)
                CurrentAbility.Name = TranslationManager.Translate($"Layer.{CurrentAbility.Type}");
        }
    }

    public ChatChannel Channels { get; set; }

    public string KilledBy { get; set; } = "";
    public DeathReasonEnum DeathReason { get; set; } = DeathReasonEnum.Alive;

    public bool Diseased { get; set; }

    public readonly List<Faction> FakeFactions = [];
    public readonly List<(Layer, Faction)> History = [];
    public readonly Dictionary<byte, PlayerArrow> AllArrows = [];
    public readonly Dictionary<byte, PlayerArrow> DeadArrows = [];

    public bool Rewinding { get; set; }
    private readonly Dictionary<float, PointInTime> Positions = [];

    private bool Local => Player.AmOwner;

    /// <summary>
    /// Gets or sets a value indicating whether or not the layer is a winner.
    /// </summary>
    public bool Winner { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the player has disconnected.
    /// </summary>
    public bool Disconnected => Player?.Data?.Disconnected ?? true;

    // private static bool PlatformIsUsed;
    // public static bool IsLeft;
    // private static bool PlayerIsLeft;
    // public CustomButton CallButton { get; set; }

    public Role CurrentRole;
    public Ability CurrentAbility;
    public Modifier CurrentModifier;
    public Disposition CurrentDisposition;
    public IEnumerable<PlayerLayer> CurrentLayers;
    public IEnumerable<CustomButton> Buttons;

    public static RoleBehaviour Crewmate;
    public static RoleBehaviour Impostor;
    public static RoleBehaviour CrewmateGhost;
    public static RoleBehaviour ImpostorGhost;

    public static Minigame HauntMenu;

    public bool Bombed;
    private CustomButton BombKillButton;

    public bool Requesting => Requestor;
    public BountyHunter Requestor;
    private CustomButton PlaceHitButton;
    private int BountyTimer;

    private bool BombUsable() => Bombed;

    private bool RequestUsable() => Requesting;

    private void PlaceHit(PlayerControl target)
    {
        target = Requestor.Player.IsLinkedTo(target) ? Player : target;
        Requestor.TentativeTarget = target;
        Requestor = null;
        CallRpc(ActionsRpc.PlaceHit, Player, target);
    }

    private void BombKill(PlayerControl target)
    {
        var success = Interact(Player, target, true) != CooldownType.Fail;
        PlayerLayer.GetLayers<Enforcer>().Where(x => x.BombedPlayer == Player).Do(x => x.BombSuccessful = success);
        CallRpc(ActionsRpc.ForceKill, Player, success);
    }

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
        TasksCountTowardProgress = Player.CanDoTasks() && (CurrentRole is Runner or Hunted || CurrentFaction == Faction.Crew);
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
        DeadArrows.Keys.Where(id => !PlayerById(id)).Do(DestroyArrowD);
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
            CallRpc(MiscRpc.WinLose, WinState, Player);
        }

        if (!TaskOptions.TasksIncreaseUses)
            return;

        foreach (var button in Buttons.Where(x => x.HasUses))
        {
            button.UsesCount++;
            button.Max++;
        }
    }

    public void OnRevive()
    {
        Channels &= ~ChatChannel.Dead;
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

        if (!Requesting || BountyTimer <= 2)
            return;

        CallRpc(ActionsRpc.PlaceHit, Player, Player);
        Requestor.TentativeTarget = Player;
        Requestor = null;
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
        Channels &= ~ChatChannel.Meeting;
        CurrentRole.OnMeetingEnd(__instance);
        CurrentAbility.OnMeetingEnd(__instance);
        CurrentModifier.OnMeetingEnd(__instance);
        CurrentDisposition.OnMeetingEnd(__instance);

        if (Requesting)
            BountyTimer++;
    }

    public void ResetButtons() => Buttons = Player.GetButtonsFromList();

    [HideFromIl2Cpp]
    public void SetUpLayers(bool inherit, PlayerLayerEnum changedLayer)
    {
        Handlers[Player.PlayerId] = this;

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

        CurrentLayers = [ CurrentRole, CurrentModifier, CurrentAbility, CurrentDisposition ];
        CurrentLayers.Do([HideFromIl2Cpp] (x) => x.Handler = this);

        Channels = ChatChannel.None;

        AffectedByLightAffectors = !(CurrentAbility is Torch || !CurrentRole.AffectedByLights);

        Player.GetComponent<PlayerControlHandler>().UpdateCurrent();

        if (CurrentFaction == Faction.None || inherit)
            CurrentFaction = CurrentRole.BaseFaction;

        if (changedLayer == PlayerLayerEnum.None)
        {
            CurrentLayers.Do([HideFromIl2Cpp] (x) => x.Init());

            // if (MapPatches.CurrentMap == 4 && CustomGameOptions.CallPlatformButton)
            // {
            //     CallButton ??= new(CurrentRole, "CALL PLATFORM", "CallPlatform", AbilityTypes.Targetless, KeybindType.Quarternary, (OnClickTargetless)UsePlatform, (UsableFunc)CallUsable,
            //         (ConditionFunc)CallCondition);
            // }

            if (GameModeSettings.GameMode is not (Mode.HideAndSeek or Mode.TaskRace))
            {
                if (RoleGenManager.GetSpawnItem(Layer.Enforcer).IsActive())
                    BombKillButton ??= new(CurrentRole, "KILL", new SpriteName("BombKill"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)BombKill, (UsableFunc)BombUsable);

                if (BountyHunter.BountyHunterCanPickTargets && RoleGenManager.GetSpawnItem(Layer.BountyHunter).IsActive())
                    PlaceHitButton ??= new(CurrentRole, "PLACE HIT", new SpriteName("PlaceHit"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)PlaceHit, (UsableFunc)RequestUsable);
            }
        }
        else
        {
            CurrentLayers.Find(x => x.LayerType == changedLayer)?.Init();
            CurrentLayers.Where(x => x.LayerType != changedLayer).Do(x => x.OnLayerChanged(changedLayer));
        }

        ResetButtons();
    }

    public void Update()
    {
        if (!Timekeeper.TkExists || !CurrentRole.Alive || CurrentFaction == Faction.GameMode || (CurrentFaction == Timekeeper.TkFaction && Timekeeper.TimeRewindImmunity))
            return;

        if (!Rewinding)
        {
            Positions.TryAdd(Time.time, new(Player.transform.position));
            (from pair in Positions let seconds = Time.time - pair.Key where seconds > Timekeeper.TimeDur select pair.Key).Do(x => Positions.Remove(x));
        }
        else if (Positions.Any())
        {
            var point = Positions.Last();
            Player.CustomSnapTo(point.Value.Position);
            Positions.Remove(point.Key);
        }
        else
            Positions.Clear();
    }

    public void UpdateButtons()
    {
        try
        {
            var hud = HUD();

            hud.SabotageButton.graphic.sprite = GetSprite($"{CurrentFaction}Sabotage");
            hud.SabotageButton.graphic.SetCooldownNormalizedUvs();

            hud.ImpostorVentButton.graphic.sprite = GetSprite($"{CurrentFaction}Vent");
            hud.ImpostorVentButton.graphic.SetCooldownNormalizedUvs();

            hud.ReportButton.buttonLabelText.SetOutlineColor(CurrentRole.FactionColor);
            hud.UseButton.buttonLabelText.SetOutlineColor(CurrentRole.FactionColor);
            hud.PetButton.buttonLabelText.SetOutlineColor(CurrentRole.FactionColor);
            hud.ImpostorVentButton.buttonLabelText.SetOutlineColor(CurrentRole.FactionColor);
            hud.SabotageButton.buttonLabelText.SetOutlineColor(CurrentRole.FactionColor);

            foreach (var button in Buttons)
            {
                button.SetActive();
                button.UpdateSprite();
            }
        } catch {}
    }

    private void DestroyArrowD(byte targetPlayerId)
    {
        if (DeadArrows.Remove(targetPlayerId, out var arrow))
            arrow.Destroy();
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
        Channels |= ChatChannel.Dead;

        if (!PlayerLayer.GetLayers<IReviver>().Any())
            CurrentLayers.Do(x => x.TrulyDead |= x.Type != Layer.GuardianAngel);
    }

    public override bool DidWin(GameOverReason gameOverReason) => Winner;

    public override void SpawnTaskHeader(PlayerControl playerControl)
    {
        if (playerControl.AmOwner)
            PlayerTask.GetOrCreateTask<ImportantTextTask>(playerControl).Text = "Achieve your goal!\n";
    }

    public override void AppendTaskHint(IStringBuilder taskStringBuilder) {}

    public override void Initialize(PlayerControl player)
    {
        Player = player;

        SetUpLayers(true, PlayerLayerEnum.None);

        IntroSound = GetAudio($"{CurrentRole}Intro", false) ?? GetAudio($"{(CurrentRole is Intruder or Syndicate or Apocalypse ? "Impostor" : "Crewmate")}Intro");

        InitializeAbilityButton();

        if (player.AmOwner && !TutorialManager.InstanceExists && !TownOfUsReworked.MciActive)
            CustomStatsManager.IncrementStat(CurrentFaction.IsOutcast() ? Faction.Outcast : CurrentFaction);
    }

    public override void OnMeetingStart()
    {
        Channels |= ChatChannel.Meeting;
        var meeting = Meeting();
        CurrentRole.OnMeetingStart(meeting);
        CurrentAbility.OnMeetingStart(meeting);
        CurrentModifier.OnMeetingStart(meeting);
        CurrentDisposition.OnMeetingStart(meeting);

        if (!Local)
            return;

        CurrentRole.LocalOnMeetingStart(meeting);
        CurrentAbility.LocalOnMeetingStart(meeting);
        CurrentModifier.LocalOnMeetingStart(meeting);
        CurrentDisposition.LocalOnMeetingStart(meeting);
    }

    public override void OnVotingComplete()
    {
        if (!Local)
            return;

        var meeting = Meeting();
        CurrentRole.VoteComplete(meeting);
        CurrentAbility.VoteComplete(meeting);
        CurrentModifier.VoteComplete(meeting);
        CurrentDisposition.VoteComplete(meeting);
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        CurrentRole.End();
        CurrentAbility.End();
        CurrentModifier.End();
        CurrentDisposition.End();

        AllArrows.Values.DestroyAll();
        AllArrows.Clear();
        DeadArrows.Values.DestroyAll();
        DeadArrows.Clear();
    }

    public override bool CanUse(IUsable console)
    {
        // This is such a cheesy way to handle this omg
        var isCrew = CurrentFaction == Faction.Crew || CurrentFaction.IsOutcast() || (CurrentFaction == Faction.GameMode && CurrentRole.Type != Layer.Hunter);
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

    private const string CrewWinCon = "- Finish all tasks\n- Eject all <#FF0000FF>evildoers</color>";

    private static string IntrudersWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Kill anyone who opposes the <#FF0000FF>Intruders</color>";

    private static string SyndicateWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Cause chaos and kill off anyone who opposes the <#008000FF>Syndicate</color>";

    private static string ApocalypseWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Summon your deities to bring on the <#99007FFF>Apocalypse</color>";

    private static string ComplianceWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Eliminate any and all opposition to the <#5A27CCFF>Compliance</color>";

    private static string PandoricaWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Kill off anyone who tries to oppose the <#ECFF45FF>Pandorica</color>";

    private static string IlluminatiWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Eliminate anyone who tries to oppose the <#A39389FF>Illuminati</color>";

    private static string CabalWinCon() => "- Kill or recruit all opposition to the <#575757FF>Cabal</color>";

    private static string CultWinCon() => "- Eliminate or persuade all opposition to the <#F995FCFF>Cult</color>";

    private static string FollowersWinCon() => "- Kill or recruit all opposition to the <#575757FF>Followers</color>";

    private static string ReanimatedWinCon() => "- Eliminate and/or reanimate all opposition to the <#917AC0FF>Reanimated</color>";

    private static string UndeadWinCon() => "- Kill or drain all opposition to the <#7B8968FF>Undead</color>";

    /*private bool CallCondition() => IsLeft == PlayerIsLeft && !PlatformIsUsed && MapPatches.CurrentMap != 4;

    private bool CallUsable()
    {
        if (MapPatches.CurrentMap != 4)
            return false;

        var pos = Player.transform.position;

        if (pos.y is >= 8.21f and < 9.62f)
        {
            if (pos.x is <= 10.8f and >= 9.7f)
            {
                PlayerIsLeft = false;
                return true;
            }
            else if (pos.x is <= 5.8f and >= 4.7f)
            {
                PlayerIsLeft = true;
                return true;
            }
        }

        return false;
    }

    private static void UsePlatform()
    {
        if (!PlatformIsUsed && LocalRole.CanCall() && LocalRole.CallUsable())
            UsePlatForRpc();
    }

    private static void UsePlatForRpc()
    {
        SyncPlatform();
        CallRpc(CustomRPC.Misc, MiscRPC.SyncPlatform);
    }

    public static void SyncPlatform() => Coroutines.Start(CoUsePlatform());

    private static IEnumerator CoUsePlatform()
    {
        IsLeft = !IsLeft;
        var platform = UObject.FindObjectOfType<MovingPlatformBehaviour>();
        PlatformIsUsed = true;
        platform.IsLeft = IsLeft;
        platform.transform.localPosition = IsLeft ? platform.LeftPosition : platform.RightPosition;
        platform.IsDirty = true;

        var sourcePos = IsLeft ? platform.LeftPosition : platform.RightPosition;
        var targetPos = IsLeft ? platform.RightPosition : platform.LeftPosition;

        yield return Effects.Wait(0.1f);
        yield return Effects.Slide3D(platform.transform, sourcePos, targetPos, LocalPlayer.MyPhysics.Speed);
        yield return Effects.Wait(0.1f);

        PlatformIsUsed = false;
    }*/
}