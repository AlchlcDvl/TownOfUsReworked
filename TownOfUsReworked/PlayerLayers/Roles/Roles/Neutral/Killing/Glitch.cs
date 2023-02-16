using Hazel;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Glitch : Role, IVisualAlteration
    {
        public PlayerControl ClosestPlayer;
        public DateTime LastMimic { get; set; }
        public DateTime LastHack { get; set; }
        public DateTime LastKilled { get; set; }
        private KillButton _hackButton { get; set; }
        private KillButton _mimicButton { get; set; }
        private KillButton _killButton { get; set; }
        public PlayerControl HackTarget { get; set; }
        public ChatController MimicList { get; set; }
        public float TimeRemaining;
        public bool IsUsingMimic => TimeRemaining > 0f;
        public bool MimicEnabled;
        public PlayerControl MimicTarget { get; set; }
        public bool GlitchWins { get; set; }
        public bool LastMouse;
        public bool MenuClick;
        public float TimeRemaining2;
        public bool IsUsingHack => TimeRemaining2 > 0f;
        public bool HackEnabled;
		public bool lastMouse;

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "Glitch";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
            MimicList = null;
            RoleType = RoleEnum.Glitch;
            StartText = "foreach PlayerControl Glitch.MurderPlayer";
            AbilitiesText = "- You can mimic players' appearances whenever you want to.\n- You can hack players to stop them from using their abilities.\n- Hacking blocks your target " +
                "from being able to use their abilities for a short while.\n- You are immune to blocks.\n- If you block a <color=#336EFFFF>Serial Killer</color>, they will be forced " +
                "to kill you.";
            Faction = Faction.Neutral;
            FactionName = "Neutral";
            FactionColor = Colors.Neutral;
            RoleAlignment = RoleAlignment.NeutralKill;
            AlignmentName = "Neutral (Killing)";
            //IntroSound = TownOfUsReworked.GlitchIntro;
            MenuClick = false;
            RoleDescription = "You are a Glitch! You are an otherworldly being who only seeks destruction. Mess with the player's systems so that they are " +
                "unable to oppose you and mimic others to frame them! Do not let anyone live.";
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
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (CustomGameOptions.NoSolo == NoSolo.AllNeutrals)
            {
                if (Utils.AllNeutralsWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.AllNeutralsWin);
                    writer.Write(Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (CustomGameOptions.NoSolo == NoSolo.AllNKs)
            {
                if (Utils.AllNKsWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.AllNeutralsWin);
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
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsPersuaded)
            {
                if (Utils.SectWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.SectWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsBitten)
            {
                if (Utils.UndeadWin())
                {
                    Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.UndeadWin);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    Utils.EndGame();
                    return false;
                }
            }
            else if (IsResurrected)
            {
                if (Utils.ReanimatedWin())
                {
                   Wins();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable, -1);
                    writer.Write((byte)WinLoseRPC.ReanimatedWin);
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
            else if (IsPersuaded)
                SectWin = true;
            else if (IsBitten)
                UndeadWin = true;
            else if (IsResurrected)
                ReanimatedWin = true;
            else if (CustomGameOptions.NoSolo == NoSolo.AllNeutrals)
                AllNeutralsWin = true;
            else if (CustomGameOptions.NoSolo == NoSolo.AllNKs)
                NKWins = true;
            else
                GlitchWins = true;
        }

        public float HackTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastHack;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.HackCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            if (Player != PlayerControl.LocalPlayer)
                return;
                
            var team = new Il2CppSystem.Collections.Generic.List<PlayerControl>();

            team.Add(PlayerControl.LocalPlayer);

            if (IsRecruit)
            {
                var jackal = Player.GetJackal();

                team.Add(jackal.Player);
                team.Add(jackal.GoodRecruit);
            }

            __instance.teamToShow = team;
        }

        public float MimicTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMimic;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.MimicCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.GlitchKillCooldown) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;

            if (flag2)
                return 0;

            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton MimicButton
        {
            get => _mimicButton;
            set
            {
                _mimicButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public KillButton HackButton
        {
            get => _hackButton;
            set
            {
                _hackButton = value;
                AddToAbilityButtons(value, this);
            }
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                AddToAbilityButtons(value, this);
            }
        }

		public void Mimic()
		{
			MimicEnabled = true;
			TimeRemaining -= Time.deltaTime;
			Utils.Morph(Player, MimicTarget);

			if (Player.Data.IsDead)
				TimeRemaining = 0f;
		}

		public void UnMimic()
		{
			MimicEnabled = false;
			MimicTarget = null;
            LastMimic = DateTime.UtcNow;
			Utils.DefaultOutfit(Player);
		}

		public void Hack()
		{
			HackEnabled = true;
			TimeRemaining2 -= Time.deltaTime;

            var targetRole = GetRole(HackTarget);
            targetRole.IsBlocked = !targetRole.RoleBlockImmune;

			if (Player.Data.IsDead)
				TimeRemaining = 0f;
		}

		public void Unhack()
		{
			HackEnabled = false;
			HackTarget = null;
            LastHack = DateTime.UtcNow;
			Utils.DefaultOutfit(Player);
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
                                    if (!Input.GetMouseButtonDown(0) && lastMouse)
                                    {
                                        lastMouse = false;
                                        MimicList.Toggle();
                                        MimicList.SetVisible(false);
                                        MimicList = null;
                                        break;
                                    }

                                    lastMouse = Input.GetMouseButtonDown(0);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void MimicButtonPress(Glitch __gInstance)
        {
            if (__gInstance.MimicList == null)
            {
                HudManager.Instance.Chat.SetVisible(false);
                __gInstance.MimicList = Object.Instantiate(HudManager.Instance.Chat);

                __gInstance.MimicList.transform.SetParent(Camera.main.transform);
                __gInstance.MimicList.SetVisible(true);
                __gInstance.MimicList.Toggle();

                __gInstance.MimicList.TextBubble.enabled = false;
                __gInstance.MimicList.TextBubble.gameObject.SetActive(false);

                __gInstance.MimicList.TextArea.enabled = false;
                __gInstance.MimicList.TextArea.gameObject.SetActive(false);

                __gInstance.MimicList.BanButton.enabled = false;
                __gInstance.MimicList.BanButton.gameObject.SetActive(false);

                __gInstance.MimicList.CharCount.enabled = false;
                __gInstance.MimicList.CharCount.gameObject.SetActive(false);

                __gInstance.MimicList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                __gInstance.MimicList.OpenKeyboardButton.Destroy();

                __gInstance.MimicList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                __gInstance.MimicList.gameObject.transform.GetChild(0).gameObject.SetActive(false);

                __gInstance.MimicList.BackgroundImage.enabled = false;

                foreach (var rend in __gInstance.MimicList.Content.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                    {
                        rend.enabled = false;
                        rend.gameObject.SetActive(false);
                    }
                }

                foreach (var bubble in __gInstance.MimicList.chatBubPool.activeChildren)
                {
                    bubble.enabled = false;
                    bubble.gameObject.SetActive(false);
                }

                __gInstance.MimicList.chatBubPool.activeChildren.Clear();

                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x != null && x.Data != null && x != PlayerControl.LocalPlayer && !x.Data.Disconnected))
                {
                    if (!player.Data.IsDead)
                        __gInstance.MimicList.AddChat(player, "Click here");
                    else
                    {
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deadBodies)
                        {
                            if (body.ParentId == player.PlayerId)
                            {
                                player.Data.IsDead = false;
                                __gInstance.MimicList.AddChat(player, "Click here");
                                player.Data.IsDead = true;
                            }
                        }
                    }
                }
            }
            else
            {
                __gInstance.MimicList.Toggle();
                __gInstance.MimicList.SetVisible(false);
                __gInstance.MimicList = null;
            }
        }
    }
}