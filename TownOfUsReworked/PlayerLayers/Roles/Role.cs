namespace TownOfUsReworked.PlayerLayers.Roles;

// TODO: Refactor the usage of linked disposition and stuff and have other ways to check players' allegiances, like for example an external dictionary for modification and stuff
public abstract class Role : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;
    public override LayerEnum Type => LayerEnum.NoneRole;
    protected override UColor LayerColor => CustomColorManager.Role;
    protected override bool UseMainColor => false;

    public virtual Func<string> StartText => () => "Woah The Game Started";
    public virtual bool RoleBlockImmune => false;
    public virtual float VisionRange => 1f;
    public virtual bool AffectedByLights => true;
    public virtual bool CanSwitchVents => true;

    public virtual List<PlayerControl> Team()
    {
        var team = new List<PlayerControl>() { Player };

        if (Player.Is<Lovers>())
            team.Add(Player.GetOtherLover());
        else if (Player.Is<Rivals>())
            team.Add(Player.GetOtherRival());
        else if (Player.Is<Mafia>())
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is<Mafia>()));

        if (SubFaction == SubFaction.Cabal && Alignment != Alignment.Neophyte)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        return team;
    }

    // private static bool PlatformIsUsed;
    // public static bool IsLeft;
    // private static bool PlayerIsLeft;
    // public CustomButton CallButton { get; set; }

    // public static bool RoleWins => WinState is WinLose.SerialKillerWins or WinLose.ArsonistWins or WinLose.CryomaniacWins or WinLose.MurdererWins or WinLose.BetrayerWins or
    //     WinLose.PhantomWins or WinLose.WerewolfWins or WinLose.ActorWins or WinLose.BountyHunterWins or WinLose.CannibalWins or WinLose.TrollWins or WinLose.ExecutionerWins or
    //     WinLose.GuesserWins or WinLose.JesterWins or WinLose.TaskRunnerWins or WinLose.HuntedWin or WinLose.HunterWins;

    // public static bool FactionWins => WinState is WinLose.CrewWins or WinLose.IntrudersWin or WinLose.SyndicateWins or WinLose.AllNeutralsWin;

    // public static bool SubFactionWins => WinState is WinLose.ApocalypseWins or WinLose.AllNKsWin or WinLose.CabalWins or WinLose.ReanimatedWins or WinLose.CultWins or WinLose.UndeadWins;

    public Alignment Alignment { get; protected set; }
    public ChatChannel CurrentChannel { get; set; }
    public LayerEnum LinkedDisposition { get; set; }

    public Dictionary<byte, PlayerArrow> AllArrows { get; } = [];
    public Dictionary<byte, PlayerArrow> DeadArrows { get; } = [];
    private Dictionary<float, PointInTime> Positions { get; } = [];
    public Dictionary<byte, PlayerArrow> YellerArrows { get; } = [];

    public List<LayerEnum> RoleHistory { get; } = [];

    private Faction FactionPriv;
    public Faction Faction
    {
        get => FactionPriv;
        set
        {
            FactionColor = value switch
            {
                Faction.Intruder => CustomColorManager.Intruder,
                Faction.Crew => CustomColorManager.Crew,
                Faction.Syndicate => CustomColorManager.Syndicate,
                Faction.Neutral => CustomColorManager.Neutral,
                Faction.Pandorica => CustomColorManager.Pandorica,
                Faction.Compliance => CustomColorManager.Compliance,
                Faction.Illuminati => CustomColorManager.Illuminati,
                Faction.GameMode => Alignment switch
                {
                    Alignment.HideAndSeek => CustomColorManager.HideAndSeek,
                    Alignment.TaskRace => CustomColorManager.TaskRace,
                    _ => CustomColorManager.Faction
                },
                _ => CustomColorManager.Faction
            };
            Objectives = value switch
            {
                Faction.Intruder => () => IntrudersWinCon,
                Faction.Syndicate => () => SyndicateWinCon,
                Faction.Crew => () => CrewWinCon,
                _ => Objectives
            };
            FactionPriv = value;
        }
    }
    public UColor FactionColor { get; private set; }
    public string FactionColorString => $"<#{FactionColor.ToHtmlStringRGBA()}>";
    public virtual string FactionName => $"{Faction}";

    private SubFaction SubFactionPriv;
    public SubFaction SubFaction
    {
        get => SubFactionPriv;
        set
        {
            SubFactionPriv = value;
            (SubFactionColor, SubFactionSymbol) = value switch
            {
                SubFaction.Undead => (CustomColorManager.Undead, "γ"),
                SubFaction.Cult => (CustomColorManager.Cult, "Λ"),
                SubFaction.Cabal => (CustomColorManager.Cabal, "$"),
                SubFaction.Reanimated => (CustomColorManager.Reanimated, "Σ"),
                _ => (CustomColorManager.SubFaction, "φ")
            };
        }
    }
    public string SubFactionSymbol { get; private set; }
    public UColor SubFactionColor { get; private set; }
    public string SubFactionColorString => $"<#{SubFactionColor.ToHtmlStringRGBA()}>";
    public string SubFactionName => $"{SubFaction}";

    public Func<string> Objectives { get; set; } = () => "- None";

    public string KilledBy { get; set; } = "";
    public DeathReasonEnum DeathReason { get; set; } = DeathReasonEnum.Alive;

    public bool Rewinding { get; set; }

    public bool Bombed { get; set; }
    private CustomButton BombKillButton { get; set; }

    public bool Requesting { get; set; }
    public PlayerControl Requestor { get; set; }
    private CustomButton PlaceHitButton { get; set; }
    private int BountyTimer { get; set; }

    private bool TrulyDeadPriv;
    public bool TrulyDead
    {
        get=> TrulyDeadPriv;
        set
        {
            TrulyDeadPriv = value;
            OnTrueDeath();
        }
    }

    public bool Diseased { get; set; }

    public bool IsRecruit => SubFaction == SubFaction.Cabal;
    // public bool IsResurrected => SubFaction == SubFaction.Reanimated;
    // public bool IsPersuaded => SubFaction == SubFaction.Cult;
    // public bool IsBitten => SubFaction == SubFaction.Undead;
    // public bool IsIntTraitor => LinkedDisposition == LayerEnum.Traitor && Faction == Faction.Intruder;
    // public bool IsIntAlly => LinkedDisposition == LayerEnum.Allied && Faction == Faction.Intruder;
    // public bool IsIntFanatic => LinkedDisposition == LayerEnum.Fanatic && Faction == Faction.Intruder;
    // public bool IsSynTraitor => LinkedDisposition == LayerEnum.Traitor && Faction == Faction.Syndicate;
    // public bool IsSynAlly => LinkedDisposition == LayerEnum.Allied && Faction == Faction.Syndicate;
    // public bool IsSynFanatic => LinkedDisposition == LayerEnum.Fanatic && Faction == Faction.Syndicate;
    // public bool IsCrewAlly => LinkedDisposition == LayerEnum.Allied && Faction == Faction.Crew;
    public bool IsCrewDefect => LinkedDisposition == LayerEnum.Defector && Faction == Faction.Crew && this is not Crew;
    private bool IsIntDefect => LinkedDisposition == LayerEnum.Defector && Faction == Faction.Intruder && this is not Intruder;
    private bool IsSynDefect => LinkedDisposition == LayerEnum.Defector && Faction == Faction.Syndicate && this is not Syndicate;
    private bool IsNeutDefect => LinkedDisposition == LayerEnum.Defector && Faction == Faction.Neutral && this is not Neutral;
    public bool Faithful => SubFaction == SubFaction.None && LinkedDisposition is not (LayerEnum.Allied or LayerEnum.Corrupted or LayerEnum.Mafia) && !IsCrewDefect && !IsIntDefect && !IsSynDefect
        && !IsNeutDefect && !Player.IsWinningRival() && !Player.HasAliveLover() && !Player.IsTurnedFanatic() && !Player.IsTurnedTraitor() && !Deinitialised;

    protected override void Init()
    {
        Faction = Faction.None;
        SubFaction = SubFaction.None;
        CurrentChannel = ChatChannel.All;

        RoleHistory.Clear();
        AllArrows.Clear();
        DeadArrows.Clear();
        Positions.Clear();
        YellerArrows.Clear();

        // if (MapPatches.CurrentMap == 4 && CustomGameOptions.CallPlatformButton)
        // {
        //     CallButton ??= new(this, "CALL PLATFORM", "CallPlatform", AbilityTypes.Targetless, KeybindType.Quarternary, (OnClickTargetless)UsePlatform, (UsableFunc)CallUsable,
        //         (ConditionFunc)CallCondition);
        // }

        if (GameModeSettings.GameMode is GameMode.HideAndSeek or GameMode.TaskRace)
            return;

        if (RoleGenManager.GetSpawnItem(LayerEnum.Enforcer).IsActive())
            BombKillButton ??= new(this, "KILL", new SpriteName("BombKill"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)BombKill, (UsableFunc)BombUsable);

        if (RoleGenManager.GetSpawnItem(LayerEnum.BountyHunter).IsActive() && BountyHunter.BountyHunterCanPickTargets)
            PlaceHitButton ??= new(this, "PLACE HIT", new SpriteName("PlaceHit"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)PlaceHit, (UsableFunc)RequestUsable);
    }

    public void UpdateButtons()
    {
        try
        {
            var hud = HUD();
            hud.SabotageButton.graphic.sprite = GetSprite(Faction switch
            {
                Faction.Syndicate => "SyndicateSabotage",
                Faction.Intruder => "IntruderSabotage",
                _ => "IntruderSabotage"
            });
            hud.SabotageButton.graphic.SetCooldownNormalizedUvs();
            hud.ImpostorVentButton.graphic.sprite = GetSprite(Faction switch
            {
                Faction.Syndicate => "SyndicateVent",
                Faction.Intruder => "IntruderVent",
                Faction.Crew => "CrewVent",
                Faction.Neutral => "NeutralVent",
                _ => "IntruderVent"
            });
            hud.ImpostorVentButton.graphic.SetCooldownNormalizedUvs();
            hud.ReportButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.UseButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.PetButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.ImpostorVentButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.SabotageButton.buttonLabelText.SetOutlineColor(FactionColor);
        } catch {}
    }

    public override void OnIntroEnd() => UpdateButtons();

    public override void UpdateHud(HudManager __instance) => DeadArrows.Keys.Where(id => !PlayerById(id)).ForEach(DestroyArrowD);

    public override void UpdatePlayer()
    {
        if (!Timekeeper.TkExists || Dead || (Faction is Faction.Syndicate && Timekeeper.TimeRewindImmunity) || Faction == Faction.GameMode)
            return;

        if (!Rewinding)
        {
            Positions.TryAdd(Time.time, new(Player.transform.position));
            (from pair in Positions let seconds = Time.time - pair.Key where seconds > Timekeeper.TimeDur select pair.Key).ForEach(x => Positions.Remove(x));
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

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (killer != Player)
        {
            KilledBy = " By " + killer.name;
            DeathReason = Meeting() ? DeathReasonEnum.Guessed : reason2;
        }
        else
            DeathReason = Meeting() ? DeathReasonEnum.Misfire : DeathReasonEnum.Suicide;

        if (!GetLayers<IReviver>().Any())
            TrulyDead |= Type != LayerEnum.GuardianAngel;
    }

    protected override void CheckWin()
    {
        if (Faithful && Faction == Faction.Neutral && Alignment == Alignment.Killing && (SameNkWins(Type) || SoloNkWins(Player)))
        {
            WinState = Type switch
            {
                LayerEnum.Arsonist => WinLose.ArsonistWins,
                LayerEnum.Cryomaniac => WinLose.CryomaniacWins,
                LayerEnum.Glitch => WinLose.GlitchWins,
                LayerEnum.Juggernaut => WinLose.JuggernautWins,
                LayerEnum.Murderer => WinLose.MurdererWins,
                LayerEnum.SerialKiller => WinLose.SerialKillerWins,
                LayerEnum.Werewolf => WinLose.WerewolfWins,
                _ => WinLose.None,
            };

            foreach (var role2 in GetLayers<Neutral>().Where(x => x.Type == Type))
            {
                if (!role2.Disconnected && role2.Faithful)
                    role2.Winner = true;
            }

            CallRpc(CustomRPC.WinLose, WinState, this);
        }
        else if (Type == LayerEnum.Betrayer && Faction == Faction.Neutral && BetrayerWins())
        {
            WinState = WinLose.BetrayerWins;
            CallRpc(CustomRPC.WinLose, WinLose.BetrayerWins);
        }
    }

    private bool BombUsable() => Bombed;

    private bool RequestUsable() => Requesting;

    public virtual void Reset(bool meeting, bool start)
    {
        if (Requesting && !start)
            BountyTimer++;
    }

    protected virtual void OnTrueDeath() {}

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
        yield return Effects.Slide3D(platform.transform, sourcePos, targetPos, CustomPlayer.Local.MyPhysics.Speed);
        yield return Effects.Wait(0.1f);

        PlatformIsUsed = false;
    }*/

    public void DestroyArrowY(byte targetPlayerId)
    {
        if (YellerArrows.Remove(targetPlayerId, out var arrow))
            arrow.Destroy();
    }

    private void DestroyArrowD(byte targetPlayerId)
    {
        if (DeadArrows.Remove(targetPlayerId, out var arrow))
            arrow.Destroy();
    }

    public override void OnMeetingEnd(MeetingHud __instance) => GetLayers<Werewolf>().ForEach(x => x.Rounds++);

    protected override void Deinit() => RoleHistory.Clear();

    public override void UpdateMap(MapBehaviour __instance)
    {
        __instance.ColorControl.baseColor = Color;
        __instance.ColorControl.SetColor(Color);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        GetLayers<Role>().ForEach(x => x.CurrentChannel = ChatChannel.All);
        GetLayers<Arsonist>().ForEach(x => x.Doused.Clear());

        if (Requesting && BountyTimer > 2)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, Player);
            Requestor.GetLayer<BountyHunter>().TentativeTarget = Player;
            Requesting = false;
            Requestor = null;
        }

        foreach (var bh in GetLayers<BountyHunter>())
        {
            if (bh.TargetPlayer || !bh.TentativeTarget || bh.Assigned)
                continue;

            bh.TargetPlayer = bh.TentativeTarget;
            bh.Assigned = true;

            // Ensures only the Bounty Hunter sees this
            if (bh.Local)
                Run("<#B51E39FF>〖 Bounty Hunt 〗</color>", "Your bounty has been received! Prepare to hunt.");
        }

        foreach (var dict in GetLayers<Dictator>())
        {
            dict.ToBeEjected = null;
            dict.Tribunal = false;
        }

        foreach (var cryo in GetLayers<Cryomaniac>())
        {
            cryo.FreezeUsed = false;
            cryo.Doused.Clear();
        }
    }

    public override void ClearArrows()
    {
        AllArrows.Values.DestroyAll();
        AllArrows.Clear();
        YellerArrows.Values.DestroyAll();
        YellerArrows.Clear();
        DeadArrows.Values.DestroyAll();
        DeadArrows.Clear();
    }

    public const string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds\n- Kill anyone who opposes the <#FF0000FF>Intruders</color>";
    public static string SyndicateWinCon => (SyndicateSettings.AltImps ? "- Have a critical sabotage reach 0 seconds\n" : "") + "- Cause chaos and kill off anyone who opposes the " +
        "<#008000FF>Syndicate</color>";
    public const string CrewWinCon = "- Finish all tasks\n- Eject all <#FF0000FF>evildoers</color>";

    private void PlaceHit(PlayerControl target)
    {
        target = Requestor.IsLinkedTo(target) ? Player : target;
        Requestor.GetLayer<BountyHunter>().TentativeTarget = target;
        Requesting = false;
        Requestor = null;
        CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, target);
    }

    public static void PublicReveal(PlayerControl player)
    {
        if (!player.Is<IRevealer>(out var revealer))
            return;

        revealer.Revealed = true;
        revealer.OnReveal();
        Flash(revealer.Color);
        BreakShield(player, true);
        GetLayers<ITrapper>().ForEach(x => x.Trapped.Remove(player.PlayerId));
    }

    public static void BreakShield(PlayerControl player, bool flag)
    {
        foreach (var role2 in GetLayers<IShielder>())
        {
            if (role2.ShieldedPlayer != player)
                continue;

            if ((role2.Local && Medic.WhoGetsNotification == ShieldOptions.Medic) || Medic.WhoGetsNotification == ShieldOptions.Everyone || (player.AmOwner && Medic.WhoGetsNotification ==
                ShieldOptions.Shielded))
            {
                var roleEffectAnimation = UObject.Instantiate(GetRoleAnim("ProtectAnim"), player.gameObject.transform);
                roleEffectAnimation.SetMaskLayerBasedOnWhoShouldSee(true);
                roleEffectAnimation.Play(player, null, player.cosmetics.FlipX, RoleEffectAnimation.SoundType.Global);
                Flash(role2.Color);
            }

            if (!flag)
                continue;

            role2.ShieldedPlayer = null;
            role2.ShieldBroken = true;

            if (TownOfUsReworked.MciActive)
                Message(player.name + " Is Now Ex-Shielded");
        }
    }

    public static void BastionBomb(Vent vent, bool flag)
    {
        foreach (var role2 in GetLayers<IVentBomber>())
        {
            if (role2.BombedIDs.Contains(vent.Id) && role2.Local)
                Flash(role2.Color);

            if (flag)
                role2.BombedIDs.Remove(vent.Id);
        }
    }

    private void BombKill(PlayerControl target)
    {
        var success = Interact(Player, target, true) != CooldownType.Fail;
        GetLayers<Enforcer>().Where(x => x.BombedPlayer == Player).ForEach(x => x.BombSuccessful = success);
        CallRpc(CustomRPC.Action, ActionsRPC.ForceKill, Player, success);
    }

    public static IEnumerable<Role> GetRoles(Faction faction) => GetLayers<Role>().Where(x => x.Faction == faction && !x.Deinitialised);

    public static IEnumerable<Role> GetRoles(Alignment ra) => GetLayers<Role>().Where(x => x.Alignment == ra && !x.Deinitialised);

    public static IEnumerable<Role> GetRoles(SubFaction subfaction) => GetLayers<Role>().Where(x => x.SubFaction == subfaction && !x.Deinitialised);
}