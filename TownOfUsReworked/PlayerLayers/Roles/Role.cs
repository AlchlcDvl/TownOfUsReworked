namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Role : PlayerLayer
    {
        public static readonly List<Role> AllRoles = new();
        public static readonly List<GameObject> Buttons = new();
        public static readonly List<PlayerControl> Cleaned = new();

        public static Role LocalRole => GetRole(PlayerControl.LocalPlayer);

        public virtual void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance) {}

        public static bool UndeadWin;
        public static bool CabalWin;
        public static bool ReanimatedWin;
        public static bool SectWin;
        public static bool InfectorsWin;

        public static bool NKWins;

        public static bool CrewWin;
        public static bool IntruderWin;
        public static bool SyndicateWin;
        public static bool AllNeutralsWin;

        public static bool GlitchWins;
        public static bool JuggernautWins;
        public static bool SerialKillerWins;
        public static bool ArsonistWins;
        public static bool CryomaniacWins;
        public static bool MurdererWins;
        public static bool WerewolfWins;

        public static bool PhantomWins;

        public static bool JesterWins;
        public static bool ActorWins;
        public static bool ExecutionerWins;
        public static bool GuesserWins;
        public static bool BountyHunterWins;
        public static bool CannibalWins;
        public static bool TrollWins;

        public static bool RoleWins => UndeadWin || CabalWin || InfectorsWin || ReanimatedWin || SectWin || NKWins || CrewWin || IntruderWin || SyndicateWin || AllNeutralsWin || GlitchWins
            || JuggernautWins || SerialKillerWins || ArsonistWins || CryomaniacWins || MurdererWins || PhantomWins || WerewolfWins || ActorWins || BountyHunterWins || CannibalWins ||
            ExecutionerWins || GuesserWins || JesterWins || TrollWins;

        public static int ChaosDriveMeetingTimerCount;
        public static bool SyndicateHasChaosDrive;
        public static PlayerControl DriveHolder;

        public Color32 FactionColor = Colors.Faction;
        public Color32 SubFactionColor = Colors.SubFaction;
        public Faction Faction = Faction.None;
        public Faction BaseFaction = Faction.None;
        public RoleAlignment RoleAlignment = RoleAlignment.None;
        public SubFaction SubFaction = SubFaction.None;
        public InspectorResults InspectorResults = InspectorResults.None;
        public List<Role> RoleHistory = new();
        public ChatChannel CurrentChannel = ChatChannel.All;
        public List<Vent> Vents = new();
        public List<Footprint> AllPrints = new();
        public Dictionary<byte, CustomArrow> AllArrows = new();
        public Dictionary<byte, CustomArrow> DeadArrows = new();
        public Dictionary<PointInTime, DateTime> Positions = new();
        public List<PointInTime> PointsInTime => Positions.Keys.ToList();
        public Dictionary<byte, CustomArrow> YellerArrows = new();

        public bool Ignore;

        public string FactionColorString => $"<color=#{FactionColor.ToHtmlStringRGBA()}>";
        public string SubFactionColorString => $"<color=#{SubFactionColor.ToHtmlStringRGBA()}>";

        public string IntroSound => $"{Name}Intro";
        public bool IntroPlayed;

        public Func<string> StartText = () => "Woah The Game Started";
        public Func<string> AbilitiesText = () => "- None";
        public Func<string> Objectives = () => "- None";

        public string FactionName => $"{Faction}";
        public string SubFactionName => $"{SubFaction}";
        public string SubFactionSymbol = "φ";

        public string KilledBy = "";
        public DeathReasonEnum DeathReason = DeathReasonEnum.Alive;

        public bool RoleBlockImmune;

        public bool Rewinding;

        public bool Bombed;
        public bool Diseased;

        public CustomButton BombKillButton;

        public bool TrulyDead;

        public bool IsRecruit;
        public bool IsResurrected;
        public bool IsPersuaded;
        public bool IsBitten;
        public bool IsIntTraitor;
        public bool IsIntAlly;
        public bool IsIntFanatic;
        public bool IsSynTraitor;
        public bool IsSynAlly;
        public bool IsSynFanatic;
        public bool IsCrewAlly;
        public bool IsCrewDefect;
        public bool IsIntDefect;
        public bool IsSynDefect;
        public bool IsNeutDefect;
        public bool Faithful => !IsRecruit && !IsResurrected && !IsPersuaded && !IsBitten && !IsIntAlly && !IsIntFanatic && !IsIntTraitor && !IsSynAlly && !IsSynTraitor && !IsSynFanatic &&
            !IsCrewAlly && !IsCrewDefect && !IsIntDefect && !IsSynDefect && !IsNeutDefect && !Player.Is(ObjectifierEnum.Corrupted) && !Player.Is(ObjectifierEnum.Mafia);

        public override void UpdateHud(HudManager __instance)
        {
            base.UpdateHud(__instance);
            __instance.ReportButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.UseButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.PetButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.ImpostorVentButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.SabotageButton.buttonLabelText.SetOutlineColor(FactionColor);
            __instance.SabotageButton.gameObject.SetActive(BaseFaction == Faction && !MeetingHud.Instance && !Player.inVent && !Player.inMovingPlat && ((Faction == Faction.Intruder &&
                CustomGameOptions.IntrudersCanSabotage) || (Faction == Faction.Syndicate && CustomGameOptions.AltImps)));
            Player.RegenTask();

            foreach (var pair in DeadArrows)
            {
                var player = Utils.PlayerById(pair.Key);
                pair.Value.Update(player.transform.position);
            }

            foreach (var yeller in Modifier.GetModifiers<Yeller>(ModifierEnum.Yeller))
            {
                if (yeller.Player != Player)
                {
                    if (!YellerArrows.ContainsKey(yeller.PlayerId))
                        YellerArrows.Add(yeller.PlayerId, new(Player, Colors.Yeller));
                    else
                        YellerArrows[yeller.PlayerId].Update(yeller.Player.transform.position, Colors.Yeller);
                }
            }

            foreach (var pair in AllArrows)
            {
                var player = Utils.PlayerById(pair.Key);
                var body = Utils.BodyById(pair.Key);

                if (player.Data.Disconnected || (player.Data.IsDead && !body))
                {
                    DestroyArrowR(pair.Key);
                    continue;
                }

                pair.Value.Update(player.Data.IsDead ? player.transform.position : body.transform.position);
            }

            BombKillButton.Update("KILL", true, Bombed);

            if (__instance.TaskPanel)
            {
                if (Player.CanDoTasks())
                {
                    var tabText = __instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>();
                    var color = "FF0000FF";

                    if (TasksDone)
                        color = "00FF00FF";
                    else if (TasksCompleted > 0)
                        color = "FFFF00FF";

                    tabText.SetText($"Tasks <color=#{color}>({TasksCompleted}/{TotalTasks})</color>");
                }
                else
                    __instance.TaskPanel.tab.transform.FindChild("TabText_TMP").GetComponent<TextMeshPro>().SetText("Fake Tasks");
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

                        if (seconds > CustomGameOptions.TimeControlDuration + 1)
                            toBeRemoved.Add(pair.Key);
                    }

                    foreach (var key in toBeRemoved)
                        Positions.Remove(key);
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

        public void DestroyArrowR(byte targetPlayerId)
        {
            var arrow = AllArrows.FirstOrDefault(x => x.Key == targetPlayerId);
            arrow.Value?.Destroy();
            AllArrows.Remove(arrow.Key);
        }

        public override void OnMeetingEnd(MeetingHud __instance)
        {
            base.OnMeetingEnd(__instance);

            if (Player.Is(ObjectifierEnum.Lovers))
                CurrentChannel = ChatChannel.Lovers;
            else if (Player.Is(ObjectifierEnum.Rivals))
                CurrentChannel = ChatChannel.Rivals;
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
            var player = Utils.PlayerByVoteArea(voteArea);
            return player == null || player.Data.Disconnected || !CustomGameOptions.LighterDarker;
        }

        public static void GenButton(PlayerVoteArea voteArea)
        {
            if (IsExempt(voteArea))
                return;

            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = UObject.Instantiate(colorButton, voteArea.transform);
            var playerControl = Utils.PlayerByVoteArea(voteArea);
            newButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite(ColorUtils.LightDarkColors[playerControl.GetDefaultOutfit().ColorId]);
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

            if (CustomGameOptions.LighterDarker)
            {
                foreach (var button in Buttons)
                {
                    if (button == null)
                        continue;

                    button.SetActive(false);
                    button.Destroy();
                }

                Buttons.Clear();
                __instance.playerStates.ToList().ForEach(GenButton);
            }

            foreach (var role in AllRoles)
                role.CurrentChannel = ChatChannel.All;

            foreach (var role2 in GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                role2.PlayerNumbers.Clear();
                role2.Actives.Clear();
                role2.MoarButtons.Clear();
                role2.Selected = null;
            }

            foreach (var guesser in GetRoles<Guesser>(RoleEnum.Guesser))
            {
                guesser.HideButtons();
                guesser.OtherButtons.Clear();
            }

            foreach (var thief in GetRoles<Thief>(RoleEnum.Thief))
            {
                thief.HideButtons();
                thief.OtherButtons.Clear();
            }

            foreach (var arso in GetRoles<Arsonist>(RoleEnum.Arsonist))
            {
                arso.Doused.RemoveAll(x => Utils.PlayerById(x).Data.IsDead || Utils.PlayerById(x).Data.Disconnected);

                if (arso.IsDead)
                    arso.Doused.Clear();
            }

            foreach (var cryo in GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
            {
                cryo.Doused.RemoveAll(x => Utils.PlayerById(x).Data.IsDead || Utils.PlayerById(x).Data.Disconnected);

                if (cryo.IsDead)
                    cryo.Doused.Clear();
            }

            foreach (var pb in GetRoles<Plaguebearer>(RoleEnum.Plaguebearer))
            {
                pb.Infected.RemoveAll(x => Utils.PlayerById(x).Data.IsDead || Utils.PlayerById(x).Data.Disconnected);

                if (pb.IsDead)
                    pb.Infected.Clear();
            }

            foreach (var framer in GetRoles<Framer>(RoleEnum.Framer))
            {
                framer.Framed.RemoveAll(x => Utils.PlayerById(x).Data.IsDead || Utils.PlayerById(x).Data.Disconnected);

                if (framer.IsDead)
                    framer.Framed.Clear();
            }

            foreach (var spell in GetRoles<Spellslinger>(RoleEnum.Spellslinger))
            {
                spell.Spelled.RemoveAll(x => Utils.PlayerById(x).Data.IsDead || Utils.PlayerById(x).Data.Disconnected);

                if (spell.IsDead)
                    spell.Spelled.Clear();
            }

            foreach (var dict in GetRoles<Dictator>(RoleEnum.Dictator))
            {
                dict.Actives.Clear();
                dict.MoarButtons.Clear();
                dict.ToBeEjected.Clear();
            }
        }

        public static readonly string IntrudersWinCon = "- Have a critical sabotage reach 0 seconds\n- Kill anyone who opposes the <color=#FF0000FF>Intruders</color>";
        public static readonly string SyndicateWinCon = (CustomGameOptions.AltImps ? "- Have a critical sabotage reach 0 seconds\n" : "") + "- Cause chaos and kill off anyone who opposes "
            + "the <color=#008000FF>Syndicate</color>";
        public static readonly string CrewWinCon = "- Finish all tasks\n- Eject all <color=#FF0000FF>evildoers</color>";

        protected Role(PlayerControl player) : base(player)
        {
            if (GetRole(player))
                GetRole(player).Player = null;

            Color = Colors.Role;
            LayerType = PlayerLayerEnum.Role;
            BombKillButton = new(this, "BombKill", AbilityTypes.Direct, "ActionSecondary", BombKill);
            RoleHistory = new();
            Vents = new();
            AllPrints = new();
            AllArrows = new();
            DeadArrows = new();
            Positions = new();
            YellerArrows = new();
            AllRoles.Add(this);
        }

        public static void BreakShield(byte playerId, bool flag)
        {
            foreach (var role2 in GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (!role2.IsMedic)
                    continue;

                if (role2.ShieldedPlayer.PlayerId == playerId && ((role2.Local && (int)CustomGameOptions.NotificationShield is 0 or 2) || CustomGameOptions.NotificationShield ==
                    NotificationOptions.Everyone || (PlayerControl.LocalPlayer.PlayerId == playerId && (int)CustomGameOptions.NotificationShield is 1 or 2)))
                {
                    Utils.Flash(role2.Color);
                }
            }

            foreach (var role2 in GetRoles<Medic>(RoleEnum.Medic))
            {
                if (role2.ShieldedPlayer.PlayerId == playerId && ((role2.Local && (int)CustomGameOptions.NotificationShield is 0 or 2) || CustomGameOptions.NotificationShield ==
                    NotificationOptions.Everyone || (PlayerControl.LocalPlayer.PlayerId == playerId && (int)CustomGameOptions.NotificationShield is 1 or 2)))
                {
                    Utils.Flash(role2.Color);
                }
            }

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);

            foreach (var role2 in GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (!role2.IsMedic)
                    continue;

                if (role2.ShieldedPlayer.PlayerId == playerId)
                {
                    role2.ShieldedPlayer = null;
                    role2.ExShielded = player;
                    Utils.LogSomething(player.name + " Is Ex-Shielded");
                }
            }

            foreach (var role2 in GetRoles<Medic>(RoleEnum.Medic))
            {
                if (role2.ShieldedPlayer.PlayerId == playerId)
                {
                    role2.ShieldedPlayer = null;
                    role2.ExShielded = player;
                    Utils.LogSomething(player.name + " Is Ex-Shielded");
                }
            }
        }

        public void BombKill()
        {
            if (Utils.IsTooFar(Player, BombKillButton.TargetPlayer) || !Bombed)
                return;

            var success = Utils.Interact(Player, BombKillButton.TargetPlayer, true)[3];
            Player.GetEnforcer().BombSuccessful = success;
            Bombed = false;
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.ForceKill);
            writer.Write(PlayerId);
            writer.Write(success);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static Role GetRole(PlayerControl player) => AllRoles.Find(x => x.Player == player);

        public static Role GetRoleFromName(string name) => AllRoles.Find(x => x.Name == name);

        public static T GetRole<T>(PlayerControl player) where T : Role => GetRole(player) as T;

        public static Role GetRole(PlayerVoteArea area) => GetRole(Utils.PlayerByVoteArea(area));

        public static List<Role> GetRoles(RoleEnum roletype) => AllRoles.Where(x => x.RoleType == roletype && !x.Ignore).ToList();

        public static List<T> GetRoles<T>(RoleEnum roletype) where T : Role => GetRoles(roletype).Cast<T>().ToList();

        public static List<Role> GetRoles(Faction faction) => AllRoles.Where(x => x.Faction == faction && !x.Ignore).ToList();

        public static List<Role> GetRoles(RoleAlignment ra) => AllRoles.Where(x => x.RoleAlignment == ra && !x.Ignore).ToList();

        public static List<Role> GetRoles(SubFaction subfaction) => AllRoles.Where(x => x.SubFaction == subfaction && !x.Ignore).ToList();

        public static List<Role> GetRoles(InspectorResults results) => AllRoles.Where(x => x.InspectorResults == results && !x.Ignore).ToList();
    }
}