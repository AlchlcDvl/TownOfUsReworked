using Hazel;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using Reactor.Utilities;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Glitch : Role, IVisualAlteration
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastMimic { get; set; }
        public DateTime LastHack { get; set; }
        public DateTime LastKill { get; set; }
        private KillButton _hackButton { get; set; }
        private KillButton _mimicButton { get; set; }
        private KillButton _killButton { get; set; }
        public PlayerControl HackTarget { get; set; }
        public ChatController MimicList { get; set; }
        public float TimeRemaining;
        public bool IsUsingMimic => TimeRemaining > 0f;
        public PlayerControl MimicTarget { get; set; }
        public bool GlitchWins { get; set; }
        public bool LastMouse;
        public bool MenuClick;
        public float TimeRemaining2;
        public bool IsUsingHack => TimeRemaining2 > 0f;
		public bool lastMouse;
		public PlayerControl SampledTarget;
		public bool Enabled;
		public ShapeshifterMinigame ShapeshifterMenuPrefab;
		public ShapeshifterMinigame ShapeshifterMenu;

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "Glitch";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
            MimicList = null;
            RoleType = RoleEnum.Glitch;
            StartText = "foreach PlayerControl Glitch.MurderPlayer";
            AbilitiesText = "- You can mimic players' appearances whenever you want to.\n- You can hack players to stop them from using their abilities.";
            AttributesText = "- Hacking blocks your target from being able to use their abilities for a short while.\n- You are immune to blocks.\n" +
                "- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced to kill you.";
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            Results = InspResults.MorphGliEscCons;
            //IntroSound = TownOfUsReworked.GlitchIntro;
            FactionDescription = NeutralFactionDescription;
            AlignmentDescription = NKDescription;
            Objectives = NKWinCon;
            MenuClick = false;
            RoleDescription = "You are a Glitch! You are an otherworldly being who only seeks destruction. Mess with the player's systems so that they are " +
                "unable to oppose you and mimic others to frame them! Do not let anyone live.";
        }

		public void MimicListUpdate()
		{
			if (ShapeshifterMenu == null)
				return;

			if (Minigame.Instance && ShapeshifterMenu.amClosing == Minigame.CloseState.Closing)
				PlayerControl.LocalPlayer.moveable = true;
		}

		public override void SetTarget()
		{
			if (Player == null || Player != PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)
				return;

			ClosestPlayer = GetTarget(GetUntargetables(), null);
			SetPlayerOutline(ClosestPlayer, Color);
		}

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit)
            {
                if (Utils.CabalWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CabalWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsCrewAlly)
            {
                if (Utils.CrewWins())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.CrewWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsIntAlly)
            {
                if (Utils.IntrudersWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.IntruderWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsSynAlly)
            {
                if (Utils.SyndicateWins())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SyndicateWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (Utils.NKWins(RoleType))
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                writer.Write((byte)WinLoseRPC.GlitchWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public override void Wins()
        {
            if (IsRecruit)
                CabalWin = true;
            else if (IsIntAlly)
                IntruderWin = true;
            else if (IsSynAlly)
                SyndicateWin = true;
            else if (IsCrewAlly)
                CrewWin = true;
            else
                GlitchWins = true;
        }

        /*public void MimicButtonPress()
        {
            if (MimicList == null)
            {
                MimicTarget = null;
                DestroyableSingleton<HudManager>.Instance.Chat.SetVisible(false);
                MimicList = Object.Instantiate(HudManager.Instance.Chat);
                MimicList.transform.SetParent(Camera.main.transform);
                MimicList.SetVisible(true);
                MimicList.Toggle();
                MimicList.TextBubble.enabled = false;
                MimicList.TextBubble.gameObject.SetActive(false);
                MimicList.TextArea.enabled = false;
                MimicList.TextArea.gameObject.SetActive(false);
                MimicList.BanButton.enabled = false;
                MimicList.BanButton.gameObject.SetActive(false);
                MimicList.CharCount.enabled = false;
                MimicList.CharCount.gameObject.SetActive(false);
                MimicList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                MimicList.OpenKeyboardButton.Destroy();
                MimicList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                MimicList.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                MimicList.BackgroundImage.enabled = false;

                foreach (var rend in MimicList.Content.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                    {
                        rend.enabled = false;
                        rend.gameObject.SetActive(false);
                    }
                }

                foreach (var bubble in MimicList.chatBubPool.activeChildren)
                {
                    bubble.enabled = false;
                    bubble.gameObject.SetActive(false);
                }

                MimicList.chatBubPool.activeChildren.Clear();

                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x != null && x.Data != null && x != PlayerControl.LocalPlayer && !x.Data.Disconnected))
                {
                    if (!player.Data.IsDead)
                        MimicList.AddChat(player, "Click here");
                    else
                    {
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deadBodies)
                        {
                            if (body.ParentId == player.PlayerId)
                            {
                                player.Data.IsDead = false;
                                MimicList.AddChat(player, "Click here");
                                player.Data.IsDead = true;
                            }
                        }
                    }
                }
            }
            else
            {
                MimicList.Toggle();
                MimicList.SetVisible(false);
                MimicList = null;
            }
        }*/

        public float HackTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastHack;
            var num = CustomGameOptions.HackCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float MimicTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMimic;
            var num = CustomGameOptions.MimicCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.GlitchKillCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        /*public void Update(HudManager __instance)
        {
            FixedUpdate(__instance);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public void FixedUpdate(HudManager __instance)
        {
            if (MimicList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (!MimicList.IsOpen || MeetingHud.Instance)
                {
                    MimicList.Toggle();
                    MimicList.SetVisible(false);
                    MimicList = null;
                }
                else
                {
                    foreach (var bubble in MimicList.chatBubPool.activeChildren)
                    {
                        if (!IsUsingMimic && MimicList != null)
                        {
                            Vector2 ScreenMin = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            Vector2 ScreenMax = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);

                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x)
                            {
                                if (Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y)
                                {
                                    if (!Input.GetMouseButtonDown(0) && LastMouse)
                                    {
                                        LastMouse = false;
                                        MimicList.Toggle();
                                        MimicList.SetVisible(false);
                                        MimicList = null;
                                        MimicTarget = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text).FirstOrDefault();
                                        Mimic();
                                        break;
                                    }

                                    LastMouse = Input.GetMouseButtonDown(0);
                                }
                            }
                        }
                    }
                }
            }
        }*/

        public KillButton MimicButton
        {
            get => _mimicButton;
            set
            {
                _mimicButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public KillButton HackButton
        {
            get => _hackButton;
            set
            {
                _hackButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
		public void Mimic()
		{
			if (Player == null || MimicTarget == null)
				return;

			Enabled = true;
			TimeRemaining -= Time.deltaTime;
			Utils.Morph(Player, MimicTarget);

			if (Player.Data.IsDead)
				TimeRemaining = 0f;
		}

		public void UnMimic()
		{
			if (Player == null)
				return;

			Enabled = false;
			TimeRemaining = 0f;
			MimicTarget = null;
			Utils.DefaultOutfit(Player);
		}

		public void OpenMimicList()
		{
			if (ShapeshifterMenu == null)
			{
				if (Camera.main == null)
					return;

				ShapeshifterMenu = Object.Instantiate<ShapeshifterMinigame>(ShapeshifterMenuPrefab, Camera.main.transform, false);
			}

			ShapeshifterMenu.transform.SetParent(Camera.main.transform, false);
			ShapeshifterMenu.transform.localPosition = new Vector3(0f, 0f, -50f);
			ShapeshifterMenu.Begin(null);
			PlayerControl.LocalPlayer.moveable = false;
			PlayerControl.LocalPlayer.NetTransform.Halt();
		}

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            if (IsUsingMimic)
            {
                appearance = MimicTarget.GetDefaultAppearance();
                var modifier = Modifier.GetModifier(MimicTarget);

                if (modifier is IVisualAlteration alteration)
                    alteration.TryGetModifiedAppearance(out appearance);

                return true;
            }

            appearance = Player.GetDefaultAppearance();
            return false;
        }

        public void SetHacked(PlayerControl hacked)
        {
            LastHack = DateTime.UtcNow;
            Coroutines.Start(Utils.Block(this, hacked));
        }

        public void RPCSetHacked(PlayerControl hacked)
        {
            var writer3 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
            writer3.Write((byte)ActionsRPC.ConsRoleblock);
            writer3.Write(hacked.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer3);
            SetHacked(hacked);
        }
    }
}