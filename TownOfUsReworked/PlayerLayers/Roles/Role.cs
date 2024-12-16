namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Role : PlayerLayer
{
    public static readonly List<byte> Cleaned = [];

    public static Role LocalRole => CustomPlayer.Local.GetRole();

    public override UColor Color => CustomColorManager.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;
    public override LayerEnum Type => LayerEnum.NoneRole;

    public virtual Faction BaseFaction => Faction.None;
    public virtual Func<string> StartText => () => "Woah The Game Started";
    public virtual bool RoleBlockImmune => false;

    public virtual List<PlayerControl> Team()
    {
        var team = new List<PlayerControl>() { Player };

        if (Player.Is(LayerEnum.Lovers))
            team.Add(Player.GetOtherLover());
        else if (Player.Is(LayerEnum.Rivals))
            team.Add(Player.GetOtherRival());
        else if (Player.Is(LayerEnum.Mafia))
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is(LayerEnum.Mafia)));

        if (IsRecruit)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        return team;
    }

    /*private static bool PlateformIsUsed;
    public static bool IsLeft;
    private static bool PlayerIsLeft;
    public CustomButton CallButton { get; set; }*/

    public static bool RoleWins => WinState is WinLose.SerialKillerWins or WinLose.ArsonistWins or WinLose.CryomaniacWins or WinLose.MurdererWins or WinLose.BetrayerWins or
        WinLose.PhantomWins or WinLose.WerewolfWins or WinLose.ActorWins or WinLose.BountyHunterWins or WinLose.CannibalWins or WinLose.TrollWins or WinLose.ExecutionerWins or
        WinLose.GuesserWins or WinLose.JesterWins or WinLose.TaskRunnerWins or WinLose.HuntedWin or WinLose.HunterWins;

    public static bool FactionWins => WinState is WinLose.CrewWins or WinLose.IntrudersWin or WinLose.SyndicateWins or WinLose.AllNeutralsWin;

    public static bool SubFactionWins => WinState is WinLose.ApocalypseWins or WinLose.AllNKsWin or WinLose.CabalWins or WinLose.ReanimatedWins or WinLose.SectWins or WinLose.UndeadWins;

    public static bool SyndicateHasChaosDrive { get; set; }

    private static PlayerControl _driveHolder;
    public static PlayerControl DriveHolder
    {
        get => _driveHolder;
        set
        {
            _driveHolder = value;

            if (!value)
                return;

            var syndicateRole = value.GetLayer<Syndicate>();

            if (!syndicateRole)
                return;

            syndicateRole.OnDriveReceived();

            if (value.AmOwner)
                syndicateRole.OnDriveReceivedLocal();
        }
    }

    public Alignment Alignment { get; set; }
    public SubFaction SubFaction { get; set; }
    public List<Role> RoleHistory { get; set; }
    public ChatChannel CurrentChannel { get; set; }
    public Dictionary<byte, CustomArrow> AllArrows { get; set; }
    public Dictionary<byte, CustomArrow> DeadArrows { get; set; }
    public Dictionary<PointInTime, DateTime> Positions { get; set; }
    public Dictionary<byte, CustomArrow> YellerArrows { get; set; }
    public LayerEnum LinkedDisposition { get; set; }

    private Faction _faction;
    public Faction Faction
    {
        get => _faction;
        set
        {
            _faction = value;
            Alignment = Alignment.GetNewAlignment(value);
        }
    }

    public string FactionColorString => $"<#{FactionColor.ToHtmlStringRGBA()}>";
    public string SubFactionColorString => $"<#{SubFactionColor.ToHtmlStringRGBA()}>";

    public Func<string> Objectives { get; set; } = () => "- None";

    public UColor FactionColor => Faction switch
    {
        Faction.Intruder => CustomColorManager.Intruder,
        Faction.Crew => CustomColorManager.Crew,
        Faction.Syndicate => CustomColorManager.Syndicate,
        Faction.Neutral => CustomColorManager.Neutral,
        Faction.GameMode => Alignment switch
        {
            Alignment.GameModeHideAndSeek => CustomColorManager.HideAndSeek,
            Alignment.GameModeTaskRace => CustomColorManager.TaskRace,
            _ => CustomColorManager.Faction
        },
        _ => CustomColorManager.Faction
    };
    public UColor SubFactionColor => SubFaction switch
    {
        SubFaction.Undead => CustomColorManager.Undead,
        SubFaction.Sect => CustomColorManager.Sect,
        SubFaction.Cabal => CustomColorManager.Cabal,
        SubFaction.Reanimated => CustomColorManager.Reanimated,
        _ => CustomColorManager.SubFaction
    };
    public virtual string FactionName => $"{Faction}";
    public virtual string SubFactionName => $"{SubFaction}";
    public string SubFactionSymbol => SubFaction switch
    {
        SubFaction.Cabal => "$",
        SubFaction.Sect => "Λ",
        SubFaction.Reanimated => "Σ",
        SubFaction.Undead => "γ",
        _ => "φ"
    };

    public string KilledBy { get; set; } = "";
    public DeathReasonEnum DeathReason { get; set; } = DeathReasonEnum.Alive;

    public bool Rewinding { get; set; }

    public bool Bombed { get; set; }
    public CustomButton BombKillButton { get; set; }

    public bool Requesting { get; set; }
    public PlayerControl Requestor { get; set; }
    public CustomButton PlaceHitButton { get; set; }
    public int BountyTimer { get; set; }

    public bool TrulyDead { get; set; }

    public bool Diseased { get; set; }

    public bool IsRecruit => SubFaction == SubFaction.Cabal;
    public bool IsResurrected => SubFaction == SubFaction.Reanimated;
    public bool IsPersuaded => SubFaction == SubFaction.Sect;
    public bool IsBitten => SubFaction == SubFaction.Undead;
    public bool IsIntTraitor => LinkedDisposition == LayerEnum.Traitor && Faction == Faction.Intruder;
    public bool IsIntAlly => LinkedDisposition == LayerEnum.Allied && Faction == Faction.Intruder;
    public bool IsIntFanatic => LinkedDisposition == LayerEnum.Fanatic && Faction == Faction.Intruder;
    public bool IsSynTraitor => LinkedDisposition == LayerEnum.Traitor && Faction == Faction.Syndicate;
    public bool IsSynAlly => LinkedDisposition == LayerEnum.Allied && Faction == Faction.Syndicate;
    public bool IsSynFanatic => LinkedDisposition == LayerEnum.Fanatic && Faction == Faction.Syndicate;
    public bool IsCrewAlly => LinkedDisposition == LayerEnum.Allied && Faction == Faction.Crew;
    public bool IsCrewDefect => LinkedDisposition == LayerEnum.Traitor && Faction == Faction.Crew && BaseFaction != Faction.Crew;
    public bool IsIntDefect => LinkedDisposition == LayerEnum.Defector && Faction == Faction.Intruder && BaseFaction != Faction.Intruder;
    public bool IsSynDefect => LinkedDisposition == LayerEnum.Defector && Faction == Faction.Syndicate && BaseFaction != Faction.Syndicate;
    public bool IsNeutDefect => LinkedDisposition == LayerEnum.Defector && Faction == Faction.Neutral && BaseFaction != Faction.Neutral;
    public bool Faithful => !IsRecruit && !IsResurrected && !IsPersuaded && !IsBitten && LinkedDisposition is not (LayerEnum.Allied or LayerEnum.Corrupted or LayerEnum.Mafia) &&
        !IsCrewDefect && !IsIntDefect && !IsSynDefect && !IsNeutDefect && !Player.IsWinningRival() && !Player.HasAliveLover() && BaseFaction == Faction && !Player.IsTurnedFanatic() &&
        !Player.IsTurnedTraitor() && !Ignore;

    public bool HasTarget => Type is LayerEnum.Executioner or LayerEnum.GuardianAngel or LayerEnum.Guesser or LayerEnum.BountyHunter;

    public override void Init()
    {
        Faction = Faction.None;
        SubFaction = SubFaction.None;
        CurrentChannel = ChatChannel.All;

        RoleHistory = [];
        AllArrows = [];
        DeadArrows = [];
        Positions = [];
        YellerArrows = [];

        /*if (MapPatches.CurrentMap == 4 && CustomGameOptions.CallPlatformButton)
        {
            CallButton ??= new(this, "CALL PLATFORM", "CallPlatform", AbilityTypes.Targetless, KeybindType.Quarternary, (OnClickTargetless)UsePlatform, (UsableFunc)CallUsable,
                (ConditionFunc)CallCondition);
        }*/

        if (!IsCustomHnS() && !IsTaskRace())
        {
            if (RoleGen.GetSpawnItem(LayerEnum.Enforcer).IsActive())
                BombKillButton ??= new(this, "KILL", new SpriteName("BombKill"), AbilityTypes.Alive, KeybindType.Quarternary, (OnClickPlayer)BombKill, (UsableFunc)BombUsable);

            if (RoleGen.GetSpawnItem(LayerEnum.BountyHunter).IsActive() && BountyHunter.BountyHunterCanPickTargets)
                PlaceHitButton ??= new(this, "PLACE HIT", new SpriteName("PlaceHit"), AbilityTypes.Alive, KeybindType.Quarternary, (OnClickPlayer)PlaceHit, (UsableFunc)RequestUsable);
        }
    }

    public void UpdateButtons()
    {
        HUD().SabotageButton.graphic.sprite = GetSprite(Faction switch
        {
            Faction.Syndicate => "SyndicateSabotage",
            Faction.Intruder => "IntruderSabotage",
            _ => "IntruderSabotage"
        });
        HUD().SabotageButton.graphic.SetCooldownNormalizedUvs();
        HUD().ImpostorVentButton.graphic.sprite = GetSprite(Faction switch
        {
            Faction.Syndicate => "SyndicateVent",
            Faction.Intruder => "IntruderVent",
            Faction.Crew => "CrewVent",
            Faction.Neutral => "NeutralVent",
            _ => "IntruderVent"
        });
        HUD().ImpostorVentButton.graphic.SetCooldownNormalizedUvs();
        HUD().ReportButton.buttonLabelText.SetOutlineColor(FactionColor);
        HUD().UseButton.buttonLabelText.SetOutlineColor(FactionColor);
        HUD().PetButton.buttonLabelText.SetOutlineColor(FactionColor);
        HUD().ImpostorVentButton.buttonLabelText.SetOutlineColor(FactionColor);
        HUD().SabotageButton.buttonLabelText.SetOutlineColor(FactionColor);
    }

    public override void OnIntroEnd() => UpdateButtons();

    public override void UpdateHud(HudManager __instance)
    {
        foreach (var pair in DeadArrows)
        {
            var player = PlayerById(pair.Key);

            if (!player)
                DestroyArrowD(pair.Key);
            else
                pair.Value?.Update(player.transform.position);
        }

        foreach (var yeller in GetLayers<Yeller>())
        {
            if (yeller.Player != Player)
            {
                if (!yeller.Dead)
                {
                    if (YellerArrows.TryGetValue(yeller.PlayerId, out var arrow))
                        arrow.Update(yeller.Player.transform.position, CustomColorManager.Yeller);
                    else
                        YellerArrows.Add(yeller.PlayerId, new(Player, CustomColorManager.Yeller));
                }
                else
                    DestroyArrowY(yeller.PlayerId);
            }
        }

        foreach (var pair in AllArrows)
        {
            var player = PlayerById(pair.Key);
            var body = BodyById(pair.Key);

            if (!player || player.Data.Disconnected || (player.Data.IsDead && !body))
                DestroyArrowR(pair.Key);
            else
                pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position);
        }

        if (!Dead && !(Faction == Faction.Syndicate && Timekeeper.TimeRewindImmunity) && Faction != Faction.GameMode && AllPlayers().Any(x => x.Is(LayerEnum.Timekeeper)))
        {
            if (!Rewinding)
            {
                Positions.TryAdd(new(Player.transform.position), DateTime.UtcNow);
                var toBeRemoved = new List<PointInTime>();

                foreach (var pair in Positions)
                {
                    var seconds = (DateTime.UtcNow - pair.Value).TotalSeconds;

                    if (seconds > Timekeeper.TimeDur + 1)
                        toBeRemoved.Add(pair.Key);
                }

                toBeRemoved.ForEach(x => Positions.Remove(x));
            }
            else if (Positions.Any())
            {
                var point = Positions.Keys.Last();
                Player.RpcCustomSnapTo(point.Position);
                Positions.Remove(point);
            }
            else
                Positions.Clear();
        }
    }

    public override void OnDeath(DeathReason reason, DeathReasonEnum reason2, PlayerControl killer)
    {
        if (killer != Player || (killer == Player && reason2 != DeathReasonEnum.Killed))
        {
            KilledBy = " By " + PlayerName;
            DeathReason = reason2;
        }
        else
            DeathReason = DeathReasonEnum.Suicide;

        if (!GetLayers<Altruist>().Any() && !GetLayers<Necromancer>().Any())
            TrulyDead |= Type != LayerEnum.GuardianAngel;
    }

    public bool BombUsable() => Bombed;

    public bool RequestUsable() => Requesting;

    /*private bool CallCondition() => IsLeft == PlayerIsLeft && !PlateformIsUsed && MapPatches.CurrentMap != 4;

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
        if (!PlateformIsUsed && LocalRole.CanCall() && LocalRole.CallUsable())
            UsePlateforRpc();
    }

    private static void UsePlateforRpc()
    {
        SyncPlatform();
        CallRpc(CustomRPC.Misc, MiscRPC.SyncPlatform);
    }

    public static void SyncPlatform() => Coroutines.Start(UsePlatformCoro());

    private static IEnumerator UsePlatformCoro()
    {
        IsLeft = !IsLeft;
        var platform = UObject.FindObjectOfType<MovingPlatformBehaviour>();
        PlateformIsUsed = true;
        platform.IsLeft = IsLeft;
        platform.transform.localPosition = IsLeft ? platform.LeftPosition : platform.RightPosition;
        platform.IsDirty = true;

        var sourcePos = IsLeft ? platform.LeftPosition : platform.RightPosition;
        var targetPos = IsLeft ? platform.RightPosition : platform.LeftPosition;

        yield return Effects.Wait(0.1f);
        yield return Effects.Slide3D(platform.transform, sourcePos, targetPos, CustomPlayer.Local.MyPhysics.Speed);
        yield return Effects.Wait(0.1f);

        PlateformIsUsed = false;
        yield break;
    }*/

    public void DestroyArrowR(byte targetPlayerId)
    {
        AllArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        AllArrows.Remove(targetPlayerId);
    }

    public void DestroyArrowY(byte targetPlayerId)
    {
        YellerArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        YellerArrows.Remove(targetPlayerId);
    }

    public void DestroyArrowD(byte targetPlayerId)
    {
        DeadArrows.FirstOrDefault(x => x.Key == targetPlayerId).Value?.Destroy();
        DeadArrows.Remove(targetPlayerId);
    }

    public override void OnMeetingEnd(MeetingHud __instance)
    {
        if (Player.Is(LayerEnum.Lovers))
            CurrentChannel = ChatChannel.Lovers;
        else if (Player.Is(LayerEnum.Rivals))
            CurrentChannel = ChatChannel.Rivals;
        else if (Player.Is(LayerEnum.Linked))
            CurrentChannel = ChatChannel.Linked;

        GetLayers<Werewolf>().ForEach(x => x.Rounds++);
    }

    public override void Deinit()
    {
        AllArrows.Values.ToList().DestroyAll();
        AllArrows.Clear();

        RoleHistory.Clear();
    }

    public override void UpdateMap(MapBehaviour __instance)
    {
        __instance.ColorControl.baseColor = Color;
        __instance.ColorControl.SetColor(Color);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        TrulyDead = Dead && Type is not (LayerEnum.Jester or LayerEnum.GuardianAngel);
        AllRoles().ForEach(x => x.CurrentChannel = ChatChannel.All);
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
            if (!bh.TargetPlayer && bh.TentativeTarget && !bh.Assigned)
            {
                bh.TargetPlayer = bh.TentativeTarget;
                bh.Assigned = true;

                // Ensures only the Bounty Hunter sees this
                if (HUD() && bh.Local)
                    Run("<#B51E39FF>〖 Bounty Hunt 〗</color>", "Your bounty has been received! Prepare to hunt.");
            }
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

    public const string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds\n- Kill anyone who opposes the <#FF0000FF>Intruders</color>";
    public static string SyndicateWinCon => (SyndicateSettings.AltImps ? "- Have a critical sabotage reach 0 seconds\n" : "") + "- Cause chaos and kill off anyone who opposes the " +
        "<#008000FF>Syndicate</color>";
    public const string CrewWinCon = "- Finish all tasks\n- Eject all <#FF0000FF>evildoers</color>";

    public void PlaceHit(PlayerControl target)
    {
        target = Requestor.IsLinkedTo(target) ? Player : target;
        Requestor.GetLayer<BountyHunter>().TentativeTarget = target;
        Requesting = false;
        Requestor = null;
        CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, target);
    }

    public static void PublicReveal(PlayerControl player)
    {
        var role = player.GetRole();

        if (role is Mayor mayor)
            mayor.Revealed = true;
        else if (role is Dictator dict)
            dict.Revealed = true;
        else
            return;

        Flash(role.Color);
        BreakShield(player, true);
        GetILayers<ITrapper>().ForEach(x => x.Trapped.Remove(player.PlayerId));
    }

    public static void BreakShield(PlayerControl player, bool flag)
    {
        foreach (var role2 in GetILayers<IShielder>())
        {
            if (role2.ShieldedPlayer != player)
                continue;

            if ((role2.Local && (int)Medic.WhoGetsNotification is 0 or 2) || (int)Medic.WhoGetsNotification == 3 || (player.AmOwner && (int)Medic.WhoGetsNotification is 1 or 2))
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

            if (TownOfUsReworked.IsTest)
                Message(player.Data.PlayerName + " Is Now Ex-Shielded");
        }
    }

    public static void BastionBomb(Vent vent, bool flag)
    {
        foreach (var role2 in GetILayers<IVentBomber>())
        {
            if (role2.BombedIDs.Contains(vent.Id) && role2.Local)
                Flash(role2.Color);

            if (flag)
                role2.BombedIDs.Remove(vent.Id);
        }
    }

    public void BombKill(PlayerControl target)
    {
        var success = Interact(Player, target, true) != CooldownType.Fail;
        GetLayers<Enforcer>().Where(x => x.BombedPlayer == Player).ForEach(x => x.BombSuccessful = success);
        GetLayers<PromotedGodfather>().Where(x => x.BombedPlayer == Player).ForEach(x => x.BombSuccessful = success);
        CallRpc(CustomRPC.Action, ActionsRPC.ForceKill, Player, success);
    }

    public static IEnumerable<Role> AllRoles() => AllLayers.Where(x => x.LayerType == PlayerLayerEnum.Role).Cast<Role>();

    public static IEnumerable<Role> GetRoles(Faction faction) => AllRoles().Where(x => x.Faction == faction && !x.Ignore);

    public static IEnumerable<Role> GetRoles(Alignment ra) => AllRoles().Where(x => x.Alignment == ra && !x.Ignore);

    public static IEnumerable<Role> GetRoles(SubFaction subfaction) => AllRoles().Where(x => x.SubFaction == subfaction && !x.Ignore);

    public static T LocalRoleAs<T>() where T : Role => LocalRole as T;
}