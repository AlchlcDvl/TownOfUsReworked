namespace TownOfUsReworked.PlayerLayers.Roles
{
    [HarmonyPatch]
    public class Role : PlayerLayer
    {
        public static readonly List<Role> AllRoles = new();
        public static readonly List<GameObject> Buttons = new();
        public static readonly Dictionary<int, string> LightDarkColors = new();
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

        public string FactionColorString => $"<color=#{FactionColor.ToHtmlStringRGBA()}>";
        public string SubFactionColorString => $"<color=#{SubFactionColor.ToHtmlStringRGBA()}>";

        public string IntroSound => $"{Name}Intro";
        public bool IntroPlayed;

        public string StartText = "Woah The Game Started";
        public string AbilitiesText = "- None";
        public string Objectives = "- None";

        public string FactionName => $"{Faction}";
        public string SubFactionName => $"{SubFaction}";

        public string KilledBy = "";
        public DeathReasonEnum DeathReason = DeathReasonEnum.Alive;

        public bool RoleBlockImmune;

        public bool Rewinding;

        public bool Bombed;
        public bool Diseased;
        public bool Base;

        public CustomButton BombKillButton;

        public GameObject SettingsButton;
        public bool SettingsActive;
        public Vector3 Pos;

        public GameObject RoleCardButton;
        public bool RoleCardActive;
        public TextMeshPro RoleInfo;
        public GameObject RoleCard;
        public Vector3 Pos2;

        public GameObject ZoomButton;
        public Vector3 Pos3;
        public bool Zooming;

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
            __instance.SabotageButton.gameObject.SetActive(((BaseFaction == Faction.Intruder && Faction == Faction.Intruder) || (BaseFaction == Faction.Syndicate && Faction ==
                Faction.Syndicate && CustomGameOptions.AltImps)) && Player.Data.IsImpostor());
            Player.RegenTask();
            __instance.GameSettings.text = GameSettings.Settings();

            foreach (var ret in GetRoles<Retributionist>(RoleEnum.Retributionist))
            {
                if (!ret.IsMedic)
                    continue;

                var exPlayer = ret.ExShielded;

                if (exPlayer != null)
                {
                    Utils.LogSomething(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.MyRend().material.SetFloat("_Outline", 0f);
                    ret.ExShielded = null;
                    continue;
                }

                var player = ret.ShieldedPlayer;

                if (player == null)
                    continue;

                if (player.Data.IsDead || ret.IsDead || ret.Disconnected)
                {
                    Retributionist.BreakShield(ret.PlayerId, player.PlayerId, true);
                    continue;
                }

                var showShielded = (int)CustomGameOptions.ShowShielded;

                if (showShielded is 3 || (Player == player && showShielded is 0 or 2) || (Player.Is(RoleEnum.Medic) && showShielded is 1 or 2))
                    player.MyRend().material.SetFloat("_Outline", 1f);
            }

            foreach (var medic in GetRoles<Medic>(RoleEnum.Medic))
            {
                var exPlayer = medic.ExShielded;

                if (exPlayer != null)
                {
                    Utils.LogSomething(exPlayer.name + " is ex-Shielded and unvisored");
                    exPlayer.MyRend().material.SetFloat("_Outline", 0f);
                    medic.ExShielded = null;
                    continue;
                }

                var player = medic.ShieldedPlayer;

                if (player == null)
                    continue;

                if (player.Data.IsDead || medic.IsDead || medic.Disconnected)
                {
                    Medic.BreakShield(medic.PlayerId, player.PlayerId, true);
                    continue;
                }

                var showShielded = (int)CustomGameOptions.ShowShielded;

                if (showShielded is 3 || (Player == player && showShielded is 0 or 2) || (Player.Is(RoleEnum.Medic) && showShielded is 1 or 2))
                    player.MyRend().material.SetFloat("_Outline", 1f);
            }

            foreach (var ga in GetRoles<GuardianAngel>(RoleEnum.GuardianAngel))
            {
                var player = ga.TargetPlayer;

                if (player == null)
                    continue;

                if (ga.Protecting)
                {
                    var showProtected = (int)CustomGameOptions.ShowProtect;

                    if (showProtected is 3 || (Player == player && showProtected is 0 or 2) || (Player.Is(RoleEnum.GuardianAngel) && showProtected is 1
                        or 2))
                    {
                        player.MyRend().material.SetFloat("_Outline", 1f);
                    }
                    else
                        player.MyRend().material.SetFloat("_Outline", 0f);
                }
                else if (ga.TargetPlayer.IsShielded())
                {
                    var showShielded = (int)CustomGameOptions.ShowShielded;

                    if (showShielded is 3 || (Player == player && showShielded is 0 or 2) || (Player.Is(RoleEnum.Medic) && showShielded is 1 or 2))
                        player.MyRend().material.SetFloat("_Outline", 1f);
                    else
                        player.MyRend().material.SetFloat("_Outline", 0f);
                }
                else
                    player.MyRend().material.SetFloat("_Outline", 0f);
            }

            if (IsDead)
            {
                foreach (var pair in DeadArrows)
                {
                    var player = Utils.PlayerById(pair.Key);
                    pair.Value.Update(player.transform.position);
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

            if (!SettingsButton)
            {
                SettingsButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                SettingsButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("CurrentSettings");
                SettingsButton.GetComponent<PassiveButton>().OnClick = new();
                SettingsButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => OpenSettings(__instance)));
            }

            Pos = __instance.MapButton.transform.localPosition + new Vector3(0, -0.66f, 0f);
            SettingsButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && ConstantVariables.IsNormal);
            SettingsButton.transform.localPosition = Pos;

            if (!RoleCardButton)
            {
                RoleCardButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                RoleCardButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("Help");
                RoleCardButton.GetComponent<PassiveButton>().OnClick = new();
                RoleCardButton.GetComponent<PassiveButton>().OnClick.AddListener((Action)(() => OpenRoleCard(__instance)));
            }

            Pos2 = Pos + new Vector3(0, -0.66f, 0f);
            RoleCardButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && ConstantVariables.IsNormal);
            RoleCardButton.transform.localPosition = Pos2;

            if (!ZoomButton)
            {
                ZoomButton = UObject.Instantiate(__instance.MapButton.gameObject, __instance.MapButton.transform.parent);
                ZoomButton.GetComponent<PassiveButton>().OnClick = new();
                ZoomButton.GetComponent<PassiveButton>().OnClick.AddListener(new Action(Zoom));
            }

            Pos3 = Pos2 + new Vector3(0, -0.66f, 0f);
            ZoomButton.SetActive(__instance.MapButton.gameObject.active && !(MapBehaviour.Instance && MapBehaviour.Instance.IsOpen) && ConstantVariables.IsNormal &&
                (!Player.IsPostmortal() || (Player.IsPostmortal() && Player.Caught())) && IsDead);
            ZoomButton.transform.localPosition = Pos3;
            ZoomButton.GetComponent<SpriteRenderer>().sprite = AssetManager.GetSprite(Zooming ? "Plus" : "Minus");

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

                __instance.TaskPanel.gameObject.SetActive(!RoleCardActive && !SettingsActive && !Zooming);
            }

            if (RoleCardActive)
                RoleInfo.text = Player.RoleCardInfo();

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

        public void OpenSettings(HudManager __instance)
        {
            SettingsActive = !SettingsActive;
            __instance.GameSettings.gameObject.SetActive(SettingsActive);
        }

        public void OpenRoleCard(HudManager __instance)
        {
            if (!RoleInfo)
            {
                RoleInfo = UObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                RoleInfo.enableWordWrapping = false;
                RoleInfo.transform.localScale = Vector3.one * 0.5f;
                RoleInfo.transform.localPosition = new(0, 0, -1f);
                RoleInfo.alignment = TextAlignmentOptions.Center;
                RoleInfo.gameObject.layer = 5;
            }

            if (!RoleCard)
            {
                RoleCard = new GameObject("RoleCard") { layer = 5 };
                RoleCard.AddComponent<SpriteRenderer>().sprite = AssetManager.GetSprite("RoleCard");
                RoleCard.transform.SetParent(__instance.transform);
                RoleCard.transform.localPosition = new(0, 0, 0);
                RoleCard.transform.localScale *= 4f;
                RoleCard.layer = 5;
            }

            RoleCardActive = !RoleCardActive;
            RoleInfo.text = Player.RoleCardInfo();
            RoleInfo.gameObject.SetActive(RoleCardActive);
            RoleCard.SetActive(RoleCardActive);
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
            return player?.Data.Disconnected == true || !CustomGameOptions.LighterDarker;
        }

        public static void GenButton(PlayerVoteArea voteArea)
        {
            if (IsExempt(voteArea))
                return;

            var colorButton = voteArea.Buttons.transform.GetChild(0).gameObject;
            var newButton = UObject.Instantiate(colorButton, voteArea.transform);
            var renderer = newButton.GetComponent<SpriteRenderer>();

            var playerControl = Utils.PlayerByVoteArea(voteArea);
            var ColorString = LightDarkColors[playerControl.GetDefaultOutfit().ColorId];

            if (ColorString == "lighter")
                renderer.sprite = AssetManager.GetSprite("Lighter");
            else if (ColorString == "darker")
                renderer.sprite = AssetManager.GetSprite("Darker");

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
            SettingsActive = false;
            HudManager.Instance.GameSettings.gameObject.SetActive(false);
            RoleCardActive = false;
            RoleInfo?.gameObject?.SetActive(false);
            RoleCard?.SetActive(false);
            Zooming = false;
            Camera.main.orthographicSize = 3f;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = 3f;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height);
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

                foreach (var voteArea in __instance.playerStates)
                    GenButton(voteArea);
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

            Color = Colors.Layer;
            LayerType = PlayerLayerEnum.Role;
            BombKillButton = new(this, "BombKill", AbilityTypes.Direct, "ActionSecondary", BombKill);
            AllRoles.Add(this);
        }

        public void Zoom()
        {
            Zooming = !Zooming;
            var size = Zooming ? 12f : 3f;
            Camera.main.orthographicSize = size;

            foreach (var cam in Camera.allCameras)
            {
                if (cam?.gameObject.name == "UI Camera")
                    cam.orthographicSize = size;
            }

            ResolutionManager.ResolutionChanged.Invoke((float)Screen.width / Screen.height);
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

        public static void SetColors()
        {
            LightDarkColors.Clear();
            LightDarkColors.Add(0, "darker"); // Red
            LightDarkColors.Add(1, "darker"); // Blue
            LightDarkColors.Add(2, "darker"); // Green
            LightDarkColors.Add(3, "lighter"); // Pink
            LightDarkColors.Add(4, "lighter"); // Orange
            LightDarkColors.Add(5, "lighter"); // Yellow
            LightDarkColors.Add(6, "darker"); // Black
            LightDarkColors.Add(7, "lighter"); // White
            LightDarkColors.Add(8, "darker"); // Purple
            LightDarkColors.Add(9, "darker"); // Brown
            LightDarkColors.Add(10, "lighter"); // Cyan
            LightDarkColors.Add(11, "lighter"); // Lime
            LightDarkColors.Add(12, "darker"); // Maroon
            LightDarkColors.Add(13, "lighter"); // Rose
            LightDarkColors.Add(14, "lighter"); // Banana
            LightDarkColors.Add(15, "darker"); // Grey
            LightDarkColors.Add(16, "darker"); // Tan
            LightDarkColors.Add(17, "lighter"); // Coral
            LightDarkColors.Add(18, "darker"); // Watermelon
            LightDarkColors.Add(19, "darker"); // Chocolate
            LightDarkColors.Add(20, "lighter"); // Sky Blue
            LightDarkColors.Add(21, "lighter"); // Biege
            LightDarkColors.Add(22, "lighter"); // Hot Pink
            LightDarkColors.Add(23, "lighter"); // Turquoise
            LightDarkColors.Add(24, "lighter"); // Lilac
            LightDarkColors.Add(25, "darker"); // Olive
            LightDarkColors.Add(26, "lighter"); //Azure
            LightDarkColors.Add(27, "darker"); // Plum
            LightDarkColors.Add(28, "darker"); // Jungle
            LightDarkColors.Add(29, "lighter"); // Mint
            LightDarkColors.Add(30, "lighter"); // Chartreuse
            LightDarkColors.Add(31, "darker"); // Macau
            LightDarkColors.Add(32, "darker"); // Tawny
            LightDarkColors.Add(33, "lighter"); // Gold
            LightDarkColors.Add(34, "lighter"); // Panda
            LightDarkColors.Add(35, "darker"); // Contrast
            LightDarkColors.Add(36, "darker"); // Chroma
            LightDarkColors.Add(37, "darker"); // Mantle
            LightDarkColors.Add(38, "lighter"); // Fire
            LightDarkColors.Add(39, "lighter"); // Galaxy
            LightDarkColors.Add(40, "lighter"); // Monochrome
            LightDarkColors.Add(41, "lighter"); // Rainbow
        }

        public static Role GetRole(PlayerControl player) => AllRoles.Find(x => x.Player == player);

        public static Role GetRoleFromName(string name) => AllRoles.Find(x => x.Name == name);

        public static T GetRole<T>(PlayerControl player) where T : Role => GetRole(player) as T;

        public static Role GetRole(PlayerVoteArea area) => GetRole(Utils.PlayerByVoteArea(area));

        public static List<Role> GetRoles(RoleEnum roletype) => AllRoles.Where(x => x.RoleType == roletype).ToList();

        public static List<T> GetRoles<T>(RoleEnum roletype) where T : Role => GetRoles(roletype).Cast<T>().ToList();

        public static List<Role> GetRoles(Faction faction) => AllRoles.Where(x => x.Faction == faction && x.Faithful).ToList();

        public static List<Role> GetRoles(RoleAlignment ra) => AllRoles.Where(x => x.RoleAlignment == ra && x.Faithful).ToList();

        public static List<Role> GetRoles(SubFaction subfaction) => AllRoles.Where(x => x.SubFaction == subfaction).ToList();

        public static List<Role> GetRoles(InspectorResults results) => AllRoles.Where(x => x.InspectorResults == results).ToList();
    }
}