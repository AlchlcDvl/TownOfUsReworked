namespace TownOfUsReworked.PlayerLayers.Roles;

public class Role : PlayerLayer
{
    public static readonly List<Role> AllRoles = new();
    public static readonly List<GameObject> Buttons = new();
    public static readonly List<PlayerControl> Cleaned = new();

    public static Role LocalRole => GetRole(CustomPlayer.Local);

    public override Color32 Color => Colors.Role;
    public override PlayerLayerEnum LayerType => PlayerLayerEnum.Role;

    public virtual Faction BaseFaction => Faction.None;
    public virtual Func<string> StartText => () => "Woah The Game Started";
    public virtual InspectorResults InspectorResults => InspectorResults.None;

    public virtual void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance) {}

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

    /*private static bool PlateformIsUsed;
    public static bool IsLeft;
    private static bool PlayerIsLeft;
    public CustomButton CallButton { get; set; }*/

    public static bool RoleWins => UndeadWin || CabalWin || ApocalypseWins || ReanimatedWin || SectWin || NKWins || CrewWin || IntruderWin || SyndicateWin || AllNeutralsWin || GlitchWins ||
        JuggernautWins || SerialKillerWins || ArsonistWins || CryomaniacWins || MurdererWins || PhantomWins || WerewolfWins || ActorWins || BountyHunterWins || CannibalWins || TrollWins ||
        ExecutionerWins || GuesserWins || JesterWins;

    public static int ChaosDriveMeetingTimerCount { get; set; }
    public static bool SyndicateHasChaosDrive { get; set; }
    public static PlayerControl DriveHolder { get; set; }

    public Color32 FactionColor { get; set; } = Colors.Faction;
    public Color32 SubFactionColor { get; set; } = Colors.SubFaction;
    public Faction Faction { get; set; } = Faction.None;
    public RoleAlignment RoleAlignment { get; set; } = RoleAlignment.None;
    public SubFaction SubFaction { get; set; } = SubFaction.None;
    public List<Role> RoleHistory { get; set; } = new();
    public ChatChannel CurrentChannel { get; set; } = ChatChannel.All;
    public List<Footprint> AllPrints { get; set; } = new();
    public Dictionary<byte, CustomArrow> AllArrows { get; set; } = new();
    public Dictionary<byte, CustomArrow> DeadArrows { get; set; } = new();
    public Dictionary<PointInTime, DateTime> Positions { get; set; } = new();
    public List<PointInTime> PointsInTime => Positions.Keys.ToList();
    public Dictionary<byte, CustomArrow> YellerArrows { get; set; } = new();

    public string FactionColorString => $"<color=#{FactionColor.ToHtmlStringRGBA()}>";
    public string SubFactionColorString => $"<color=#{SubFactionColor.ToHtmlStringRGBA()}>";

    public string IntroSound => $"{Name}Intro";
    public bool IntroPlayed { get; set; }

    public Func<string> Objectives { get; set; } = () => "- None";

    public string FactionName => $"{Faction}";
    public string SubFactionName => $"{SubFaction}";
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

        foreach (var yeller in Modifier.GetModifiers<Yeller>(LayerEnum.Yeller))
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

        BombKillButton.Update("KILL", true, Bombed);
        PlaceHitButton.Update("PLACE HIT", true, Requesting);
        //CallButton?.Update("CALL PLATFORM", CanCall(), IsInPosition());

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

        if (!IsDead && !(Faction == Faction.Syndicate && CustomGameOptions.TimeRewindImmunity))
        {
            if (!Rewinding)
            {
                Positions.Add(new(Player.transform.position), DateTime.UtcNow);
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

        foreach (var arso in GetRoles<Arsonist>(LayerEnum.Arsonist))
        {
            arso.Doused.RemoveAll(x => PlayerById(x).Data.IsDead || PlayerById(x).Data.Disconnected);

            if (arso.IsDead)
                arso.Doused.Clear();
        }

        foreach (var cryo in GetRoles<Cryomaniac>(LayerEnum.Cryomaniac))
        {
            cryo.Doused.RemoveAll(x => PlayerById(x).Data.IsDead || PlayerById(x).Data.Disconnected);

            if (cryo.IsDead)
                cryo.Doused.Clear();
        }

        foreach (var pb in GetRoles<Plaguebearer>(LayerEnum.Plaguebearer))
        {
            pb.Infected.RemoveAll(x => PlayerById(x).Data.IsDead || PlayerById(x).Data.Disconnected);

            if (pb.IsDead)
                pb.Infected.Clear();
        }

        foreach (var framer in GetRoles<Framer>(LayerEnum.Framer))
        {
            framer.Framed.RemoveAll(x => PlayerById(x).Data.IsDead || PlayerById(x).Data.Disconnected);

            if (framer.IsDead)
                framer.Framed.Clear();
        }

        foreach (var spell in GetRoles<Spellslinger>(LayerEnum.Spellslinger))
        {
            spell.Spelled.RemoveAll(x => PlayerById(x).Data.IsDead || PlayerById(x).Data.Disconnected);

            if (spell.IsDead)
                spell.Spelled.Clear();
        }
    }

    /*private bool CanCall() => ((IsLeft && PlayerIsLeft) || (!IsLeft && !PlayerIsLeft)) && !PlateformIsUsed && TownOfUsReworked.NormalOptions.MapId != 4;

    private bool IsInPosition()
    {
        if (TownOfUsReworked.NormalOptions.MapId != 4)
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

    private static bool IsExempt(PlayerVoteArea voteArea)
    {
        var player = PlayerByVoteArea(voteArea);
        return player == null || player.Data.Disconnected || !ClientGameOptions.LighterDarker;
    }

    public static void GenButton(PlayerVoteArea voteArea)
    {
        if (IsExempt(voteArea))
            return;

        var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
        var newButton = UObject.Instantiate(colorButton, voteArea.transform);
        var playerControl = PlayerByVoteArea(voteArea);
        newButton.GetComponent<SpriteRenderer>().sprite = GetSprite(ColorUtils.LightDarkColors[playerControl.GetDefaultOutfit().ColorId]);
        newButton.transform.position = colorButton.transform.position - new Vector3(-0.8f, 0.2f, -2f);
        newButton.transform.localScale *= 0.8f;
        newButton.layer = 5;
        newButton.transform.parent = colorButton.transform.parent.parent;
        newButton.GetComponent<PassiveButton>().OnClick = new();
        Buttons.Add(newButton);
    }

    public override void OnMeetingStart(MeetingHud __instance)
    {
        base.OnMeetingStart(__instance);
        TrulyDead = IsDead;

        if (ClientGameOptions.LighterDarker)
        {
            foreach (var button in Buttons)
            {
                if (button == null)
                    continue;

                button.SetActive(false);
                button.Destroy();
            }

            Buttons.Clear();
            AllVoteAreas.ForEach(GenButton);
        }

        AllRoles.ForEach(x => x.CurrentChannel = ChatChannel.All);
        GetRoles<Thief>(LayerEnum.Thief).ForEach(x => x.GuessMenu.HideButtons());
        GetRoles<Guesser>(LayerEnum.Guesser).ForEach(x => x.GuessMenu.HideButtons());
        GetRoles<Operative>(LayerEnum.Operative).ForEach(x => x.PlayerNumbers.Clear());

        if (Requesting && BountyTimer > 2)
        {
            CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, Player);
            GetRole<BountyHunter>(Requestor).TentativeTarget = Player;
            Requesting = false;
            Requestor = null;
        }

        foreach (var ret in GetRoles<Retributionist>(LayerEnum.Retributionist))
        {
            ret.RetMenu.HideButtons();
            ret.PlayerNumbers.Clear();
            ret.Selected = null;
        }

        foreach (var dict in GetRoles<Dictator>(LayerEnum.Dictator))
        {
            dict.DictMenu.HideButtons();
            dict.ToBeEjected.Clear();
        }

        foreach (var bh in GetRoles<BountyHunter>(LayerEnum.BountyHunter))
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

    protected Role(PlayerControl player) : base(player)
    {
        if (GetRole(player))
            GetRole(player).Player = null;

        BombKillButton = new(this, "BombKill", AbilityTypes.Direct, "Quarternary", BombKill);
        PlaceHitButton = new(this, "PlaceHit", AbilityTypes.Direct, "Quarternary", PlaceHit);
        RoleHistory = new();
        AllPrints = new();
        AllArrows = new();
        DeadArrows = new();
        Positions = new();
        YellerArrows = new();
        AllRoles.Add(this);

        /*if (TownOfUsReworked.NormalOptions.MapId == 4)
            CallButton = new(this, "CallPlatform", AbilityTypes.Effect, "Quarternary", UsePlatform);*/
    }

    public void PlaceHit()
    {
        if (IsTooFar(Player, PlaceHitButton.TargetPlayer) || !Requesting)
            return;

        var target = Requestor.IsLinkedTo(PlaceHitButton.TargetPlayer) ? Player : PlaceHitButton.TargetPlayer;
        GetRole<BountyHunter>(Requestor).TentativeTarget = target;
        Requesting = false;
        Requestor = null;
        CallRpc(CustomRPC.Action, ActionsRPC.PlaceHit, Player, target);
    }

    public static void BreakShield(PlayerControl player, bool flag)
    {
        foreach (var role2 in GetRoles<Retributionist>(LayerEnum.Retributionist))
        {
            if (!role2.IsMedic)
                continue;

            if (role2.ShieldedPlayer == null)
                continue;

            if (role2.ShieldedPlayer == player && ((role2.Local && (int)CustomGameOptions.NotificationShield is 0 or 2) || CustomGameOptions.NotificationShield ==
                ShieldOptions.Everyone || (CustomPlayer.Local == player && (int)CustomGameOptions.NotificationShield is 1 or 2)))
            {
                Flash(role2.Color);
            }
        }

        foreach (var role2 in GetRoles<Medic>(LayerEnum.Medic))
        {
            if (role2.ShieldedPlayer == null)
                continue;

            if (role2.ShieldedPlayer == player && ((role2.Local && (int)CustomGameOptions.NotificationShield is 0 or 2) || CustomGameOptions.NotificationShield ==
                ShieldOptions.Everyone || (CustomPlayer.Local == player && (int)CustomGameOptions.NotificationShield is 1 or 2)))
            {
                Flash(role2.Color);
            }
        }

        if (!flag)
            return;

        foreach (var role2 in GetRoles<Retributionist>(LayerEnum.Retributionist))
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

        foreach (var role2 in GetRoles<Medic>(LayerEnum.Medic))
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

    public void BombKill()
    {
        if (IsTooFar(Player, BombKillButton.TargetPlayer) || !Bombed)
            return;

        var success = Interact(Player, BombKillButton.TargetPlayer, true)[3];
        GetRoles<Enforcer>(LayerEnum.Enforcer).Where(x => x.BombedPlayer == Player).ToList().ForEach(x =>
        {
            x.BombSuccessful = success;
            x.TimeRemaining = 0;
            x.Unboom();
        });
        CallRpc(CustomRPC.Action, ActionsRPC.ForceKill, Player, success);
    }

    public static Role GetRole(PlayerControl player) => AllRoles.Find(x => x.Player == player);

    public static T GetRole<T>(PlayerControl player) where T : Role => GetRole(player) as T;

    public static Role GetRole(PlayerVoteArea area) => GetRole(PlayerByVoteArea(area));

    public static List<Role> GetRoles(LayerEnum roletype) => AllRoles.Where(x => x.Type == roletype && !x.Ignore && x.LayerType == PlayerLayerEnum.Role).ToList();

    public static List<T> GetRoles<T>(LayerEnum roletype) where T : Role => GetRoles(roletype).Cast<T>().ToList();

    public static List<Role> GetRoles(Faction faction) => AllRoles.Where(x => x.Faction == faction && !x.Ignore).ToList();

    public static List<Role> GetRoles(RoleAlignment ra) => AllRoles.Where(x => x.RoleAlignment == ra && !x.Ignore).ToList();

    public static List<Role> GetRoles(SubFaction subfaction) => AllRoles.Where(x => x.SubFaction == subfaction && !x.Ignore).ToList();

    public static List<Role> GetRoles(InspectorResults results) => AllRoles.Where(x => x.InspectorResults == results && !x.Ignore).ToList();
}