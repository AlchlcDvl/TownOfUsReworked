namespace TownOfUsReworked.PlayerLayers.Roles;

public abstract class Role : PlayerLayer
{
    public static readonly List<Role> AllRoles = new();
    public static readonly List<PlayerControl> Cleaned = new();

    public static Role LocalRole => GetRole(CustomPlayer.Local);

    public override Color Color => Colors.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;

    public virtual Faction BaseFaction => Faction.None;
    public virtual Func<string> StartText => () => "Woah The Game Started";

    public virtual List<PlayerControl> Team() => new() { Player };

    public static bool UndeadWin { get; set; }
    public static bool CabalWin { get; set; }
    public static bool ReanimatedWin { get; set; }
    public static bool SectWin { get; set; }

    public static bool ApocalypseWins { get; set; }

    public static bool NKWins { get; set; }

    public static bool CrewWin { get; set; }
    public static bool IntruderWin { get; set; }
    public static bool SyndicateWin { get; set; }
    public static bool AllNeutralsWin { get; set; }

    public static bool GlitchWins { get; set; }
    public static bool JuggernautWins { get; set; }
    public static bool SerialKillerWins { get; set; }
    public static bool ArsonistWins { get; set; }
    public static bool CryomaniacWins { get; set; }
    public static bool MurdererWins { get; set; }
    public static bool WerewolfWins { get; set; }

    public static bool PhantomWins { get; set; }

    public static bool JesterWins { get; set; }
    public static bool ActorWins { get; set; }
    public static bool ExecutionerWins { get; set; }
    public static bool GuesserWins { get; set; }
    public static bool BountyHunterWins { get; set; }
    public static bool CannibalWins { get; set; }
    public static bool TrollWins { get; set; }

    public static bool TaskRunnerWins { get; set; }

    public static bool HuntedWins { get; set; }
    public static bool HunterWins { get; set; }

    /*private static bool PlateformIsUsed;
    public static bool IsLeft;
    private static bool PlayerIsLeft;
    public CustomButton CallButton { get; set; }*/

    public static bool RoleWins => UndeadWin || CabalWin || ApocalypseWins || ReanimatedWin || SectWin || NKWins || CrewWin || IntruderWin || SyndicateWin || AllNeutralsWin || GlitchWins ||
        JuggernautWins || SerialKillerWins || ArsonistWins || CryomaniacWins || MurdererWins || PhantomWins || WerewolfWins || ActorWins || BountyHunterWins || CannibalWins || TrollWins ||
        ExecutionerWins || GuesserWins || JesterWins || TaskRunnerWins || HuntedWins || HunterWins;

    public static bool FactionWins => CrewWin || IntruderWin || SyndicateWin || AllNeutralsWin;

    public static bool SubFactionWins => UndeadWin || CabalWin || ReanimatedWin || SectWin;

    public static int ChaosDriveMeetingTimerCount { get; set; }
    public static bool SyndicateHasChaosDrive { get; set; }
    public static PlayerControl DriveHolder { get; set; }

    public Color FactionColor { get; set; } = Colors.Faction;
    public Color SubFactionColor { get; set; } = Colors.SubFaction;
    public Faction Faction { get; set; } = Faction.None;
    public Alignment Alignment { get; set; } = Alignment.None;
    public SubFaction SubFaction { get; set; } = SubFaction.None;
    public List<Role> RoleHistory { get; set; }
    public ChatChannel CurrentChannel { get; set; } = ChatChannel.All;
    public Dictionary<byte, CustomArrow> AllArrows { get; set; }
    public Dictionary<byte, CustomArrow> DeadArrows { get; set; }
    public Dictionary<PointInTime, DateTime> Positions { get; set; }
    public List<PointInTime> PointsInTime => Positions.Keys.ToList();
    public Dictionary<byte, CustomArrow> YellerArrows { get; set; }
    public Dictionary<byte, TMP_Text> PlayerNumbers { get; set; }

    public string FactionColorString => $"<color=#{FactionColor.ToHtmlStringRGBA()}>";
    public string SubFactionColorString => $"<color=#{SubFactionColor.ToHtmlStringRGBA()}>";

    public Func<string> Objectives { get; set; } = () => "- None";

    public virtual string FactionName => $"{Faction}";
    public virtual string SubFactionName => $"{SubFaction}";
    public string SubFactionSymbol { get; set; } = "φ";

    public string KilledBy { get; set; } = "";
    public DeathReasonEnum DeathReason { get; set; } = DeathReasonEnum.Alive;

    public bool RoleBlockImmune { get; set; }

    public bool Rewinding { get; set; }

    public bool Bombed { get; set; }
    public CustomButton BombKillButton { get; set; }

    public bool Requesting { get; set; }
    public PlayerControl Requestor { get; set; }
    public CustomButton PlaceHitButton { get; set; }
    public int BountyTimer { get; set; }

    public bool TrulyDead { get; set; }

    public bool Diseased { get; set; }

    public bool IsRecruit { get; set; }
    public bool IsResurrected { get; set; }
    public bool IsPersuaded { get; set; }
    public bool IsBitten { get; set; }
    public bool IsIntTraitor { get; set; }
    public bool IsIntAlly { get; set; }
    public bool IsIntFanatic { get; set; }
    public bool IsSynTraitor { get; set; }
    public bool IsSynAlly { get; set; }
    public bool IsSynFanatic { get; set; }
    public bool IsCrewAlly { get; set; }
    public bool IsCrewDefect { get; set; }
    public bool IsIntDefect { get; set; }
    public bool IsSynDefect { get; set; }
    public bool IsNeutDefect { get; set; }
    public bool Faithful => !IsRecruit && !IsResurrected && !IsPersuaded && !IsBitten && !Player.Is(LayerEnum.Allied) && !IsCrewDefect && !IsIntDefect && !IsSynDefect && !IsNeutDefect &&
        !Player.Is(LayerEnum.Corrupted) && !Player.Is(LayerEnum.Mafia) && !Player.IsWinningRival() && !Player.HasAliveLover() && BaseFaction == Faction && !Player.IsTurnedFanatic() &&
        !Player.IsTurnedTraitor() && !Ignore;

    public bool HasTarget => Type is LayerEnum.Executioner or LayerEnum.GuardianAngel or LayerEnum.Guesser or LayerEnum.BountyHunter;

    protected Role(PlayerControl player) : base(player)
    {
        if (GetRole(player))
            GetRole(player).Player = null;

        RoleHistory = new();
        AllArrows = new();
        DeadArrows = new();
        Positions = new();
        YellerArrows = new();
        PlayerNumbers = new();
        AllRoles.Add(this);

        /*if (MapPatches.CurrentMap == 4 && CustomGameOptions.CallPlatformButton)
            CallButton = new(this, "CallPlatform", AbilityTypes.Targetless, "Quarternary", UsePlatform);*/

        if (!IsCustomHnS && !IsTaskRace)
        {
            if (CustomGameOptions.EnforcerOn > 0)
                BombKillButton = new(this, "BombKill", AbilityTypes.Target, "Quarternary", BombKill);

            if (CustomGameOptions.BountyHunterOn > 0 && CustomGameOptions.BountyHunterCanPickTargets)
                PlaceHitButton = new(this, "PlaceHit", AbilityTypes.Target, "Quarternary", PlaceHit);
        }
    }

    public override void UpdateHud(HudManager __instance)
    {
        base.UpdateHud(__instance);
        __instance.ReportButton.buttonLabelText.SetOutlineColor(FactionColor);
        __instance.UseButton.buttonLabelText.SetOutlineColor(FactionColor);
        __instance.PetButton.buttonLabelText.SetOutlineColor(FactionColor);
        __instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(FactionColor);
        __instance.SabotageButton.buttonLabelText.SetOutlineColor(FactionColor);

        foreach (var pair in DeadArrows)
        {
            var player = PlayerById(pair.Key);

            if (player == null)
                DestroyArrowD(pair.Key);
            else
                pair.Value?.Update(player.transform.position);
        }

        foreach (var yeller in GetLayers<Yeller>())
        {
            if (yeller.Player != Player)
            {
                if (!yeller.IsDead)
                {
                    if (!YellerArrows.ContainsKey(yeller.PlayerId))
                        YellerArrows.Add(yeller.PlayerId, new(Player, Colors.Yeller));
                    else
                        YellerArrows[yeller.PlayerId].Update(yeller.Player.transform.position, Colors.Yeller);
                }
                else
                    DestroyArrowY(yeller.PlayerId);
            }
        }

        foreach (var pair in AllArrows)
        {
            var player = PlayerById(pair.Key);
            var body = BodyById(pair.Key);

            if (player == null || player.Data.Disconnected || (player.Data.IsDead && !body))
                DestroyArrowR(pair.Key);
            else
                pair.Value?.Update(player.Data.IsDead ? body.transform.position : player.transform.position);
        }

        BombKillButton?.Update2("KILL", Bombed);
        PlaceHitButton?.Update2("PLACE HIT", Requesting);
        //CallButton?.Update("CALL PLATFORM", IsInPosition(), CanCall());

        if (__instance.TaskPanel)
        {
            var tabText = __instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>();
            var text = "";

            if (Player.CanDoTasks())
            {
                var color = "FF0000FF";

                if (TasksDone)
                    color = "00FF00FF";
                else if (TasksCompleted > 0)
                    color = "FFFF00FF";

                text = $"Tasks <color=#{color}>({TasksCompleted}/{TotalTasks})</color>";
            }
            else
                text = "Fake Tasks";

            tabText.SetText(text);
        }

        if (!IsDead && !(Faction == Faction.Syndicate && CustomGameOptions.TimeRewindImmunity) && Faction != Faction.GameMode)
        {
            if (!Rewinding)
            {
                Positions.TryAdd(new(Player.transform.position), DateTime.UtcNow);
                var toBeRemoved = new List<PointInTime>();

                foreach (var pair in Positions)
                {
                    var seconds = (DateTime.UtcNow - pair.Value).TotalSeconds;

                    if (seconds > CustomGameOptions.TimeDur + 1)
                        toBeRemoved.Add(pair.Key);
                }

                toBeRemoved.ForEach(x => Positions.Remove(x));
            }
            else if (Positions.Count > 0)
            {
                var point = PointsInTime[^1];
                Player.NetTransform.RpcSnapTo(point.Position);
                Positions.Remove(point);
            }
            else
                Positions.Clear();
        }
    }

    /*private bool CanCall() => ((IsLeft && PlayerIsLeft) || (!IsLeft && !PlayerIsLeft)) && !PlateformIsUsed && MapPatches.CurrentMap != 4;

    private bool IsInPosition()
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
        if (!PlateformIsUsed && LocalRole.CanCall() && LocalRole.IsInPosition())
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
        base.OnMeetingEnd(__instance);

        if (Player.Is(LayerEnum.Lovers))
            CurrentChannel = ChatChannel.Lovers;
        else if (Player.Is(LayerEnum.Rivals))
            CurrentChannel = ChatChannel.Rivals;
        else if (Player.Is(LayerEnum.Linked))
            CurrentChannel = ChatChannel.Linked;
    }

    public override void OnLobby()
    {
        base.OnLobby();
        AllArrows.Values.ToList().DestroyAll();
        AllArrows.Clear();
    }

    public override void UpdateMap(MapBehaviour __instance)
    {
        base.UpdateMap(__instance);
        __instance.ColorControl.baseColor = Color;
        __instance.ColorControl.SetColor(Color);

        if (IsBlocked)
            __instance.Close();
    }

    public void GenText(PlayerVoteArea voteArea)
    {
        if (PlayerNumbers.TryGetValue(voteArea.TargetPlayerId, out var nameText))
        {
            nameText?.gameObject?.SetActive(false);
            nameText?.gameObject?.Destroy();
            PlayerNumbers.Remove(voteArea.TargetPlayerId);
        }

        nameText = UObject.Instantiate(voteArea.NameText, voteArea.transform);
        nameText.transform.localPosition = new(-0.911f, -0.18f, -0.1f);
        nameText.text = "";
        nameText.name = "SubTextLabel";
        nameText.color = UColor.white;
        PlayerNumbers.Add(voteArea.TargetPlayerId, nameText);
        GenNumbers(voteArea);
        GenLighterDarker(voteArea);
    }

    public void GenNumbers(PlayerVoteArea voteArea)
    {
        if (!PlayerNumbers.TryGetValue(voteArea.TargetPlayerId, out var nameText))
            return;

        if ((DataManager.Settings.Accessibility.ColorBlindMode && Type is LayerEnum.Operative or LayerEnum.Retributionist) || CustomGameOptions.Whispers)
            nameText.text = $"{voteArea.TargetPlayerId} ";
        else
            nameText.text = nameText.text.Replace($"{voteArea.TargetPlayerId} ", "");
    }

    public void GenLighterDarker(PlayerVoteArea voteArea)
    {
        if (!PlayerNumbers.TryGetValue(voteArea.TargetPlayerId, out var nameText))
            return;

        var playerControl = PlayerByVoteArea(voteArea);
        var ld = CustomColors.LightDarkColors[playerControl.GetDefaultOutfit().ColorId] == "Lighter" ? "L" : "D";

        if (ClientGameOptions.LighterDarker)
            nameText.text += $"({ld})";
        else
            nameText.text = nameText.text.Replace($"({ld})", "");
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        TrulyDead = IsDead;
        AllVoteAreas.ForEach(GenText);
        AllRoles.ForEach(x => x.CurrentChannel = ChatChannel.All);
        GetLayers<Thief>().ForEach(x => x.GuessMenu.HideButtons());
        GetLayers<Guesser>().ForEach(x => x.GuessMenu.HideButtons());

        if (Requesting && BountyTimer > 2)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, Player);
            GetRole<BountyHunter>(Requestor).TentativeTarget = Player;
            Requesting = false;
            Requestor = null;
        }

        foreach (var ret in GetLayers<Retributionist>())
        {
            ret.RetMenu.HideButtons();
            ret.Selected = null;
        }

        foreach (var dict in GetLayers<Dictator>())
        {
            dict.DictMenu.HideButtons();
            dict.ToBeEjected.Clear();
        }

        foreach (var bh in GetLayers<BountyHunter>())
        {
            if (bh.TargetPlayer == null && bh.TentativeTarget != null && !bh.Assigned)
            {
                bh.TargetPlayer = bh.TentativeTarget;
                bh.Assigned = true;

                //Ensures only the Bounty Hunter sees this
                if (HUD && bh.Local)
                    Run(HUD.Chat, "<color=#B51E39FF>〖 Bounty Hunt 〗</color>", "Your bounty has been received! Prepare to hunt.");
            }
        }
    }

    public const string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds\n- Kill anyone who opposes the <color=#FF0000FF>Intruders</color>";
    public static string SyndicateWinCon => (CustomGameOptions.AltImps ? "- Have a critical sabotage reach 0 seconds\n" : "") + "- Cause chaos and kill off anyone who opposes the " +
        "<color=#008000FF>Syndicate</color>";
    public const string CrewWinCon = "- Finish all tasks\n- Eject all <color=#FF0000FF>evildoers</color>";

    public void PlaceHit()
    {
        var target = Requestor.IsLinkedTo(PlaceHitButton.TargetPlayer) ? Player : PlaceHitButton.TargetPlayer;
        GetRole<BountyHunter>(Requestor).TentativeTarget = target;
        Requesting = false;
        Requestor = null;
        CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, target);
    }

    public static void BreakShield(PlayerControl player, bool flag)
    {
        foreach (var role2 in GetLayers<Retributionist>())
        {
            if (!role2.IsMedic || role2.ShieldedPlayer == null)
                continue;

            if (role2.ShieldedPlayer == player && ((role2.Local && (int)CustomGameOptions.NotificationShield is 0 or 2) || (int)CustomGameOptions.NotificationShield == 3 ||
                (CustomPlayer.Local == player && (int)CustomGameOptions.NotificationShield is 1 or 2)))
            {
                Flash(role2.Color);
            }
        }

        foreach (var role2 in GetLayers<Medic>())
        {
            if (role2.ShieldedPlayer == null)
                continue;

            if (role2.ShieldedPlayer == player && ((role2.Local && (int)CustomGameOptions.NotificationShield is 0 or 2) || (int)CustomGameOptions.NotificationShield == 3 ||
                (CustomPlayer.Local == player && (int)CustomGameOptions.NotificationShield is 1 or 2)))
            {
                Flash(role2.Color);
            }
        }

        if (!flag)
            return;

        foreach (var role2 in GetLayers<Retributionist>())
        {
            if (!role2.IsMedic || role2.ShieldedPlayer == null)
                continue;

            if (role2.ShieldedPlayer == player)
            {
                role2.ShieldedPlayer = null;
                role2.ExShielded = player;

                if (TownOfUsReworked.IsTest)
                    LogMessage(player.name + " Is Ex-Shielded");
            }
        }

        foreach (var role2 in GetLayers<Medic>())
        {
            if (role2.ShieldedPlayer == null)
                continue;

            if (role2.ShieldedPlayer == player)
            {
                role2.ShieldedPlayer = null;
                role2.ExShielded = player;

                if (TownOfUsReworked.IsTest)
                    LogMessage(player.name + " Is Ex-Shielded");
            }
        }
    }

    public static void BastionBomb(Vent vent, bool flag)
    {
        foreach (var role2 in GetLayers<Bastion>())
        {
            if (role2.BombedIDs.Contains(vent.Id) && role2.Local)
                Flash(role2.Color);
        }

        foreach (var role2 in GetLayers<Retributionist>())
        {
            if (!role2.IsBast)
                continue;

            if (role2.BombedIDs.Contains(vent.Id) && role2.Local)
                Flash(role2.Color);
        }

        if (!flag)
            return;

        foreach (var role2 in GetLayers<Bastion>())
        {
            if (role2.BombedIDs.Contains(vent.Id))
                role2.BombedIDs.Remove(vent.Id);
        }

        foreach (var role2 in GetLayers<Retributionist>())
        {
            if (!role2.IsBast)
                continue;

            if (role2.BombedIDs.Contains(vent.Id))
                role2.BombedIDs.Remove(vent.Id);
        }
    }

    public void BombKill()
    {
        var success = Interact(Player, BombKillButton.TargetPlayer, true).AbilityUsed;
        GetLayers<Enforcer>().Where(x => x.BombedPlayer == Player).ForEach(x => x.BombSuccessful = success);
        GetLayers<PromotedGodfather>().Where(x => x.BombedPlayer == Player).ForEach(x => x.BombSuccessful = success);
        CallRpc(CustomRPC.Action, ActionsRPC.ForceKill, Player, success);
    }

    public static Role GetRole(PlayerControl player) => AllRoles.Find(x => x.Player == player);

    public static T GetRole<T>(PlayerControl player) where T : Role => GetRole(player) as T;

    public static Role GetRole(PlayerVoteArea area) => GetRole(PlayerByVoteArea(area));

    public static List<Role> GetRoles(LayerEnum roletype) => AllRoles.Where(x => x.Type == roletype && !x.Ignore).ToList();

    public static List<Role> GetRoles(Faction faction) => AllRoles.Where(x => x.Faction == faction && !x.Ignore).ToList();

    public static List<Role> GetRoles(Alignment ra) => AllRoles.Where(x => x.Alignment == ra && !x.Ignore).ToList();

    public static List<Role> GetRoles(SubFaction subfaction) => AllRoles.Where(x => x.SubFaction == subfaction && !x.Ignore).ToList();
}