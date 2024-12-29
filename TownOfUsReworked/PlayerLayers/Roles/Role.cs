namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Role : PlayerLayer
{
    public static readonly List<byte> Cleaned = [];

    public override UColor Color => CustomColorManager.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;
    public override LayerEnum Type => LayerEnum.NoneRole;

    public virtual Faction BaseFaction => Faction.None;
    public virtual Func<string> StartText => () => "Woah The Game Started";
    public virtual bool RoleBlockImmune => false;

    public virtual List<PlayerControl> Team()
    {
        var team = new List<PlayerControl>() { Player };

        if (Player.Is<Lovers>())
            team.Add(Player.GetOtherLover());
        else if (Player.Is<Rivals>())
            team.Add(Player.GetOtherRival());
        else if (Player.Is<Mafia>())
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is<Mafia>()));

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

    public Alignment Alignment { get; set; }
    public List<Role> RoleHistory { get; set; }
    public ChatChannel CurrentChannel { get; set; }
    public Dictionary<byte, PlayerArrow> AllArrows { get; set; }
    public Dictionary<byte, PlayerArrow> DeadArrows { get; set; }
    public Dictionary<PointInTime, float> Positions { get; set; }
    public Dictionary<byte, PlayerArrow> YellerArrows { get; set; }
    public LayerEnum LinkedDisposition { get; set; }

    private Faction _faction;
    public Faction Faction
    {
        get => _faction;
        set
        {
            _faction = value;
            FactionColor = value switch
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
            Alignment = Alignment.GetNewAlignment(value);
            Objectives = value switch
            {
                Faction.Intruder => () => IntrudersWinCon,
                Faction.Syndicate => () => SyndicateWinCon,
                Faction.Crew => () => CrewWinCon,
                _ => Objectives
            };
        }
    }
    private SubFaction _subFaction;
    public SubFaction SubFaction
    {
        get => _subFaction;
        set
        {
            _subFaction = value;
            SubFactionColor = value switch
            {
                SubFaction.Undead => CustomColorManager.Undead,
                SubFaction.Sect => CustomColorManager.Sect,
                SubFaction.Cabal => CustomColorManager.Cabal,
                SubFaction.Reanimated => CustomColorManager.Reanimated,
                _ => CustomColorManager.SubFaction
            };
            SubFactionSymbol = value switch
            {
                SubFaction.Cabal => "$",
                SubFaction.Sect => "Λ",
                SubFaction.Reanimated => "Σ",
                SubFaction.Undead => "γ",
                _ => "φ"
            };
        }
    }

    public string FactionColorString => $"<#{FactionColor.ToHtmlStringRGBA()}>";
    public string SubFactionColorString => $"<#{SubFactionColor.ToHtmlStringRGBA()}>";

    public Func<string> Objectives { get; set; } = () => "- None";

    public UColor FactionColor { get; set; }
    public UColor SubFactionColor { get; set; }
    public string SubFactionSymbol { get; set; }
    public virtual string FactionName => $"{Faction}";
    public virtual string SubFactionName => $"{SubFaction}";

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
            if (RoleGenManager.GetSpawnItem(LayerEnum.Enforcer).IsActive())
                BombKillButton ??= new(this, "KILL", new SpriteName("BombKill"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)BombKill, (UsableFunc)BombUsable);

            if (RoleGenManager.GetSpawnItem(LayerEnum.BountyHunter).IsActive() && BountyHunter.BountyHunterCanPickTargets)
                PlaceHitButton ??= new(this, "PLACE HIT", new SpriteName("PlaceHit"), AbilityTypes.Player, KeybindType.Quarternary, (OnClickPlayer)PlaceHit, (UsableFunc)RequestUsable);
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
        foreach (var id in DeadArrows.Keys)
        {
            if (!PlayerById(id))
                DestroyArrowD(id);
        }

        if (Timekeeper.TKExists && !Dead && !(Faction == Faction.Syndicate && Timekeeper.TimeRewindImmunity) && Faction != Faction.GameMode)
        {
            if (!Rewinding)
            {
                Positions.TryAdd(new(Player.transform.position), Time.time);
                var toBeRemoved = new List<PointInTime>();

                foreach (var pair in Positions)
                {
                    var seconds = Time.time - pair.Value;

                    if (seconds > Timekeeper.TimeDur)
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
        if (killer != Player)
        {
            KilledBy = " By " + killer.name;
            DeathReason = Meeting() ? DeathReasonEnum.Guessed : reason2;
        }
        else
            DeathReason = Meeting() ? DeathReasonEnum.Misfire : DeathReasonEnum.Suicide;

        if (!GetLayers<Altruist>().Any() && !GetLayers<Necromancer>().Any())
            TrulyDead |= Type != LayerEnum.GuardianAngel;
    }

    public override void CheckWin()
    {
        if ((IsRecruit || Type == LayerEnum.Jackal) && CabalWin())
        {
            WinState = WinLose.CabalWins;
            CallRpc(CustomRPC.WinLose, WinLose.CabalWins, this);
        }
        else if ((IsPersuaded || Type == LayerEnum.Whisperer) && SectWin())
        {
            WinState = WinLose.SectWins;
            CallRpc(CustomRPC.WinLose, WinLose.SectWins);
        }
        else if ((IsBitten || Type == LayerEnum.Dracula) && UndeadWin())
        {
            WinState = WinLose.UndeadWins;
            CallRpc(CustomRPC.WinLose, WinLose.UndeadWins);
        }
        else if ((IsResurrected || Type == LayerEnum.Necromancer) && ReanimatedWin())
        {
            WinState = WinLose.ReanimatedWins;
            CallRpc(CustomRPC.WinLose, WinLose.ReanimatedWins);
        }
        else if (Faction == Faction.Syndicate && (Faithful || Type == LayerEnum.Betrayer || IsSynAlly || IsSynDefect || IsSynFanatic || IsSynTraitor) && SyndicateWins())
        {
            WinState = WinLose.SyndicateWins;
            CallRpc(CustomRPC.WinLose, WinLose.SyndicateWins);
        }
        else if (Faction == Faction.Intruder && (Faithful || Type == LayerEnum.Betrayer || IsIntDefect || IsIntAlly || IsIntFanatic || IsIntTraitor) && IntrudersWin())
        {
            WinState = WinLose.IntrudersWin;
            CallRpc(CustomRPC.WinLose, WinLose.IntrudersWin);
        }
        else if (Faction == Faction.Crew && (Faithful || IsCrewAlly || IsCrewDefect) && CrewWins())
        {
            WinState = WinLose.CrewWins;
            CallRpc(CustomRPC.WinLose, WinLose.CrewWins);
        }
        else if (Faithful && ApocWins() && Alignment is Alignment.NeutralApoc or Alignment.NeutralHarb)
        {
            WinState = WinLose.ApocalypseWins;
            CallRpc(CustomRPC.WinLose, WinLose.ApocalypseWins);
        }
        else if (Faithful && Faction == Faction.Neutral && AllNeutralsWin())
        {
            WinState = WinLose.AllNeutralsWin;
            CallRpc(CustomRPC.WinLose, WinLose.AllNeutralsWin);
        }
        else if (Faithful && Alignment == Alignment.NeutralKill && AllNKsWin())
        {
            WinState = WinLose.AllNKsWin;
            CallRpc(CustomRPC.WinLose, WinLose.AllNKsWin);
        }
        else if (Faithful && Alignment == Alignment.NeutralKill && (SameNKWins(Type) || SoloNKWins(Player)))
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

            if (NeutralSettings.NoSolo == NoSolo.SameNKs)
            {
                foreach (var role2 in GetLayers<Neutral>().Where(x => x.Type == Type))
                {
                    if (!role2.Disconnected && role2.Faithful)
                        role2.Winner = true;
                }
            }

            Winner = true;
            CallRpc(CustomRPC.WinLose, WinState, this);
        }
        else if (Type == LayerEnum.Betrayer && Faction == Faction.Neutral)
        {
            WinState = WinLose.BetrayerWins;
            CallRpc(CustomRPC.WinLose, WinLose.BetrayerWins);
        }
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

    public override void OnMeetingEnd(MeetingHud __instance) => GetLayers<Werewolf>().ForEach(x => x.Rounds++);

    public override void Deinit() => RoleHistory.Clear();

    public override void UpdateMap(MapBehaviour __instance)
    {
        __instance.ColorControl.baseColor = Color;
        __instance.ColorControl.SetColor(Color);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        TrulyDead = Dead && Type is not (LayerEnum.Jester or LayerEnum.GuardianAngel);
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

    public override void ClearArrows()
    {
        AllArrows.Values.DestroyAll();
        AllArrows.Clear();
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
                Message(player.name + " Is Now Ex-Shielded");
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

    public static IEnumerable<Role> GetRoles(Faction faction) => GetLayers<Role>().Where(x => x.Faction == faction && !x.Ignore);

    public static IEnumerable<Role> GetRoles(Alignment ra) => GetLayers<Role>().Where(x => x.Alignment == ra && !x.Ignore);

    public static IEnumerable<Role> GetRoles(SubFaction subfaction) => GetLayers<Role>().Where(x => x.SubFaction == subfaction && !x.Ignore);
}