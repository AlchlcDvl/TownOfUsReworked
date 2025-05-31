namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Role : PlayerLayer
{
    protected override UColor MainColor => CustomColorManager.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;
    public override LayerEnum Type => LayerEnum.NoneRole;
    protected override UColor LayerColor => CustomColorManager.Role;
    protected override bool UseMainColor => false;

    public virtual Func<string> StartText { get; } = () => "Woah The Game Started";
    public virtual bool RoleBlockImmune => false;
    public virtual bool AffectedByLights => true;
    public virtual bool CanSwitchVents => true;

    // private static bool PlatformIsUsed;
    // public static bool IsLeft;
    // private static bool PlayerIsLeft;
    // public CustomButton CallButton { get; set; }

    public Alignment Alignment { get; protected set; }
    public ChatChannel CurrentChannel { get; set; }
    public LayerEnum LinkedDisposition { get; set; }

    public Dictionary<byte, PlayerArrow> AllArrows { get; } = [];
    public Dictionary<byte, PlayerArrow> DeadArrows { get; } = [];
    private Dictionary<float, PointInTime> Positions { get; } = [];
    public Dictionary<byte, PlayerArrow> YellerArrows { get; } = [];

    public List<LayerEnum> RoleHistory { get; } = [];

    public Faction Faction
    {
        get;
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
                Faction.Apocalypse => CustomColorManager.Apocalypse,
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
                Faction.Intruder => () => IntrudersWinCon(Player),
                Faction.Syndicate => () => SyndicateWinCon(Player),
                Faction.Apocalypse => () => ApocalypseWinCon(Player),
                Faction.Compliance => () => ComplianceWinCon(Player),
                Faction.Pandorica => () => PandoricaWinCon(Player),
                Faction.Illuminati => () => IlluminatiWinCon(Player),
                Faction.Crew => () => CrewWinCon,
                _ => Objectives
            };

            if (Local)
                UpdateButtons();

            field = value;
        }
    }
    public UColor FactionColor { get; private set; }
    public string FactionColorString => $"<#{FactionColor.ToHtmlStringRGBA()}>";
    public virtual string FactionName => $"{Faction}";

    public SubFaction SubFaction
    {
        get;
        set
        {
            (SubFactionColor, SubFactionSymbol) = value switch
            {
                SubFaction.Undead => (CustomColorManager.Undead, "γ"),
                SubFaction.Cult => (CustomColorManager.Cult, "Λ"),
                SubFaction.Cabal => (CustomColorManager.Cabal, "$"),
                SubFaction.Reanimated => (CustomColorManager.Reanimated, "Σ"),
                SubFaction.Followers => (CustomColorManager.Followers, "王"),
                _ => (CustomColorManager.SubFaction, "φ")
            };
            field = value;
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

    public bool TrulyDead
    {
        get=> field;
        set
        {
            field = value;
            OnTrueDeath();
        }
    }

    public bool Diseased { get; set; }

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

        if (GameModeSettings.GameMode is Mode.HideAndSeek or Mode.TaskRace)
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

            hud.SabotageButton.graphic.sprite = GetSprite($"{Faction}Sabotage");
            hud.SabotageButton.graphic.SetCooldownNormalizedUvs();

            hud.ImpostorVentButton.graphic.sprite = GetSprite($"{Faction}Vent");
            hud.ImpostorVentButton.graphic.SetCooldownNormalizedUvs();

            hud.ReportButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.UseButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.PetButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.ImpostorVentButton.buttonLabelText.SetOutlineColor(FactionColor);
            hud.SabotageButton.buttonLabelText.SetOutlineColor(FactionColor);

            Player.GetButtons().Do(x => x.UpdateSprite());
        } catch {}
    }

    public virtual List<PlayerControl> Team()
    {
        var team = new List<PlayerControl>() { Player };

        if (LinkedDisposition == LayerEnum.Lovers)
            team.Add(Player.GetOtherLover());
        else if (LinkedDisposition == LayerEnum.Rivals)
            team.Add(Player.GetOtherRival());
        else if (LinkedDisposition == LayerEnum.Mafia)
            team.AddRange(AllPlayers().Where(x => x != Player && x.Is<Mafia>()));

        if (SubFaction == SubFaction.Cabal && Alignment != Alignment.Neophyte)
        {
            var jackal = Player.GetJackal();
            team.Add(jackal.Player);
            team.AddRange(jackal.GetOtherRecruits(Player));
        }

        return team;
    }

    public override void OnIntroEnd() => UpdateButtons();

    public override void UpdateHud(HudManager __instance) => DeadArrows.Keys.Where(id => !PlayerById(id)).Do(DestroyArrowD);

    public override void UpdatePlayer()
    {
        if (!Timekeeper.TkExists || Dead || Faction == Faction.GameMode || (Faction is Faction.Syndicate && Timekeeper.TimeRewindImmunity))
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

    public override void OnDeath(DeathReasonEnum reason, PlayerControl killer)
    {
        if (killer != Player)
        {
            KilledBy = " By " + killer.name;
            DeathReason = Meeting() ? DeathReasonEnum.Guessed : reason;
        }
        else
            DeathReason = Meeting() ? DeathReasonEnum.Misfire : DeathReasonEnum.Suicide;

        if (!GetLayers<IReviver>().Any())
            TrulyDead |= Type != LayerEnum.GuardianAngel;
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
        yield return Effects.Slide3D(platform.transform, sourcePos, targetPos, LocalPlayer.MyPhysics.Speed);
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

    public bool IsConverted() => SubFaction != SubFaction.None && this is not Neophyte;

    public void RoleUpdate(Role former, PlayerControl player = null, bool retainFaction = false)
    {
        player ??= former.Player;
        CustomButton.AllButtons.Where(x => x.Owner == former || !x.Owner.Player).Do(x => x.Destroy());
        CustomArrow.AllArrows.Where(x => x.Owner == player).Do(x => x.Disable());
        var allArrows = former.AllArrows.Clone();
        var history = former.RoleHistory.Clone();
        former.End();
        Start(player);
        SubFaction = former.SubFaction;
        DeathReason = former.DeathReason;
        KilledBy = former.KilledBy;
        Diseased = former.Diseased;
        AllArrows.AddRange(allArrows);
        RoleHistory.AddRange(history);
        RoleHistory.Add(former.Type);

        if (!retainFaction)
            Faction = former.Faction;
        else if (Local)
            UpdateButtons();

        if (Local)
        {
            ButtonUtils.Reset();
            Player.RegenTask();
            Flash(Color);
        }

        if (LocalPlayer.Is<Seer>(out var seer))
            Flash(seer.Color);

        if (player.Data.Role is LayerHandler layerHandler)
            layerHandler.SetUpLayers();
    }

    public override void OnMeetingEnd(MeetingHud __instance) => GetLayers<Werewolf>().Do(x => x.Rounds++);

    protected override void Deinit() => RoleHistory.Clear();

    public override void UpdateMap(MapBehaviour __instance)
    {
        __instance.ColorControl.baseColor = Color;
        __instance.ColorControl.SetColor(Color);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        GetLayers<Role>().Do(x => x.CurrentChannel = ChatChannel.All);
        GetLayers<Arsonist>().Do(x => x.Doused.Clear());

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
        if (!player.Is<Sovereign>(out var revealer))
            return;

        Flash(revealer.Color);
        BreakShield(player, true);
        GetLayers<ITrapper>().Do(x => x.Trapped.Remove(player.PlayerId));
        revealer.Revealed = true;
        revealer.OnReveal();
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
        GetLayers<Enforcer>().Where(x => x.BombedPlayer == Player).Do(x => x.BombSuccessful = success);
        CallRpc(CustomRPC.Action, ActionsRPC.ForceKill, Player, success);
    }

    public static IEnumerable<Role> GetRoles(Faction faction) => GetLayers<Role>().Where(x => x.Faction == faction && !x.Deinitialised);

    public static IEnumerable<Role> GetRoles(Alignment ra) => GetLayers<Role>().Where(x => x.Alignment == ra && !x.Deinitialised);

    public static IEnumerable<Role> GetRoles(SubFaction subfaction) => GetLayers<Role>().Where(x => x.SubFaction == subfaction && !x.Deinitialised);

    public static string IntrudersWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Kill anyone who opposes the <#FF0000FF>Intruders</color>";

    public static string SyndicateWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Cause chaos and kill off anyone who opposes the <#008000FF>Syndicate</color>";

    public static string ApocalypseWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Summon your deities to bring on the <#99007FFF>Apocalypse</color>";

    public static string ComplianceWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Eliminate any and all opposition to the <#5A27CCFF>Compliance</color>";

    public static string PandoricaWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Kill off anyone who tries to oppose the <#ECFF45FF>Pandorica</color>";

    public static string IlluminatiWinCon(PlayerControl player) => (player.CanSabotage() ? "- Have a critical sabotage reach 0 seconds\n" : "") +
        "- Eliminate anyone who tries to oppose the <#A39389FF>Illuminati</color>";
}