using Hazel;
using System;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUsReworked.PlayerLayers.Roles.Roles
{
    public class Glitch : Role, IVisualAlteration
    {
        public bool lastMouse;
        public PlayerControl ClosestPlayer;
        public DateTime LastMimic { get; set; }
        public DateTime LastHack { get; set; }
        public DateTime LastKill { get; set; }
        private KillButton _glitchButton { get; set; }
        private KillButton _killButton { get; set; }
        public PlayerControl HackTarget { get; set; }
        public ChatController MimicList { get; set; }
        public float TimeRemaining;
        public bool IsUsingMimic => TimeRemaining > 0f;
        public PlayerControl MimicTarget { get; set; }
        public bool PressedButton;
        public bool GlitchWins { get; set; }

        public Glitch(PlayerControl owner) : base(owner)
        {
            Name = "Glitch";
            Color = CustomGameOptions.CustomNeutColors ? Colors.Glitch : Colors.Neutral;
            SubFaction = SubFaction.None;
            LastHack = DateTime.UtcNow;
            LastMimic = DateTime.UtcNow;
            LastKill = DateTime.UtcNow;
            HackTarget = null;
            MimicList = null;
            PressedButton = false;
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
            IntroText = "DDoS those who oppose you";
            CoronerDeadReport = "The abnormal condition of the body indicates they are not from this reality. They must be a Glitch!";
            CoronerKillerReport = "The body has been left in a very abnormal state. They were killed by a Glitch!";
            Results = InspResults.EscConsGliPois;
            Attack = AttackEnum.Basic;
            Defense = DefenseEnum.None;
            AttackString = "Basic";
            DefenseString = "None";
            IntroSound = TownOfUsReworked.GlitchIntro;
            FactionDescription = "Your faction is Neutral! You do not have any team mates and can only by yourself or by other players after finishing" +
                " a certain objective.";
            AlignmentDescription = "You are a Neutral (Killing) role! You side with no one and can only win by yourself. You have a special way to kill " +
                "and gain a large body count. Make sure no one survives.";
            Objectives = "- Kill: <color=#FF0000FF>Intruders</color>, <color=#8BFDFD>Crew</color>, <color=#008000FF>Syndicate</color> and other " +
                "<color=#B3B3B3FF>Neutral</color> <color=#1D7CF2FF>Killers</color>, <color=#1D7CF2FF>Proselytes</color> and <color=#1D7CF2FF>Neophytes</color>";
            RoleDescription = "You are a Glitch! You are an otherworldly being who only seeks destruction. Mess with the player's systems so that they are " +
                "unable to oppose you and mimic others to frame them! Do not let anyone live.";
            AddToRoleHistory(RoleType);
        }

        internal override bool EABBNOODFGL(ShipStatus __instance)
        {
            if (Player.Data.IsDead | Player.Data.Disconnected)
                return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.Intruders) |
                (x.Is(RoleAlignment.NeutralKill) && !x.Is(RoleEnum.Glitch)) | x.Is(RoleAlignment.NeutralNeo) | x.Is(RoleAlignment.NeutralPros) |
                x.Is(Faction.Crew))) == 0)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.GlitchWin,
                    SendOption.Reliable, -1);
                writer.Write(Player.PlayerId);
                Wins();
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public override void Wins()
        {
            GlitchWins = true;
        }

        public override void Loses()
        {
            LostByRPC = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
        {
            var glitchTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            glitchTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = glitchTeam;
        }

        public void FixedUpdate(HudManager __instance)
        {
            if (__instance.KillButton != null && Player.Data.IsDead)
                __instance.KillButton.SetTarget(null);

            if (GlitchButton != null && Player.Data.IsDead)
                GlitchButton.SetTarget(null);

            if (GlitchButton != null && Player.Data.IsDead)
                GlitchButton.SetTarget(null);

            if (MimicList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (!MimicList.IsOpen | MeetingHud.Instance)
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

        public void RpcSetHacked(PlayerControl hacked)
        {
            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Hack,
                    SendOption.Reliable, -1);
                writer.Write(hacked.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
        }

        public static class KillButtonHandler
        {
            public static void KillButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (!__gInstance.Player.Data.IsImpostor() && Input.GetKeyDown(KeyCode.Q))
                    __instance.KillButton.DoClick();

                __instance.KillButton.gameObject.SetActive(__instance.UseButton.isActiveAndEnabled && !MeetingHud.Instance &&
                    !__gInstance.Player.Data.IsDead);
                __instance.KillButton.SetCoolDown(__gInstance.KillTimer(), CustomGameOptions.GlitchKillCooldown);

                __instance.KillButton.SetTarget(null);
                __gInstance.ClosestPlayer = null;

                if (__instance.KillButton.isActiveAndEnabled)
                    __instance.KillButton.SetTarget(__gInstance.ClosestPlayer);

                if (__gInstance.ClosestPlayer != null)
                    __gInstance.ClosestPlayer.myRend().material.SetColor("_OutlineColor", __gInstance.Color);
            }

            public static void KillButtonPress(Glitch __gInstance, KillButton __instance)
            {
                if (__gInstance.ClosestPlayer != null)
                {
                    if (__gInstance.Player.inVent)
                        return;

                    if (__gInstance.ClosestPlayer.Is(RoleEnum.Pestilence))
                    {
                        if (__gInstance.Player.IsShielded())
                        {
                            var medic = __gInstance.Player.GetMedic().Player.PlayerId;
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer.Write(medic);
                            writer.Write(__gInstance.Player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.ShieldBreaks)
                                __gInstance.LastKill = DateTime.UtcNow;

                            StopKill.BreakShield(medic, __gInstance.Player.PlayerId,
                                CustomGameOptions.ShieldBreaks);
                        }

                        if (__gInstance.Player.IsProtected())
                        {
                            __gInstance.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset);
                            return;
                        }

                        Utils.RpcMurderPlayer(__gInstance.ClosestPlayer, __gInstance.Player);
                        return;
                    }

                    if (__gInstance.ClosestPlayer.IsInfected() | __gInstance.Player.IsInfected())
                    {
                        foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                            ((Plaguebearer)pb).RpcSpreadInfection(__gInstance.ClosestPlayer, __gInstance.Player);
                    }

                    if (__gInstance.ClosestPlayer.IsOnAlert())
                    {
                        if (__gInstance.ClosestPlayer.IsShielded())
                        {
                            var medic = __gInstance.ClosestPlayer.GetMedic().Player.PlayerId;
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer.Write(medic);
                            writer.Write(__gInstance.ClosestPlayer.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.ShieldBreaks)
                                __gInstance.LastKill = DateTime.UtcNow;

                            StopKill.BreakShield(medic, __gInstance.ClosestPlayer.PlayerId, CustomGameOptions.ShieldBreaks);

                            if (!__gInstance.Player.IsProtected())
                                Utils.RpcMurderPlayer(__gInstance.ClosestPlayer, __gInstance.Player);
                        }
                        else if (__gInstance.Player.IsShielded())
                        {
                            var medic = __gInstance.Player.GetMedic().Player.PlayerId;
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                            writer.Write(medic);
                            writer.Write(__gInstance.Player.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);

                            if (CustomGameOptions.ShieldBreaks)
                                __gInstance.LastKill = DateTime.UtcNow;

                            StopKill.BreakShield(medic, __gInstance.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                        }
                        else if (__gInstance.ClosestPlayer.IsProtected())
                            Utils.RpcMurderPlayer(__gInstance.ClosestPlayer, __gInstance.Player);
                        else
                            Utils.RpcMurderPlayer(__gInstance.ClosestPlayer, __gInstance.Player);

                        return;
                    }
                    else if (__gInstance.ClosestPlayer.IsShielded())
                    {
                        var medic = __gInstance.ClosestPlayer.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                            (byte)CustomRPC.AttemptSound, SendOption.Reliable, -1);
                        writer.Write(medic);
                        writer.Write(__gInstance.ClosestPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            __gInstance.LastKill = DateTime.UtcNow;

                        StopKill.BreakShield(medic, __gInstance.ClosestPlayer.PlayerId,
                            CustomGameOptions.ShieldBreaks);

                        return;
                    }
                    else if (__gInstance.ClosestPlayer.IsVesting())
                    {
                        __gInstance.LastKill.AddSeconds(CustomGameOptions.VestKCReset);

                        return;
                    }
                    else if (__gInstance.ClosestPlayer.IsProtected())
                    {
                        __gInstance.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset);

                        return;
                    }

                    __gInstance.LastKill = DateTime.UtcNow;
                    __gInstance.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown);
                    Utils.RpcMurderPlayer(__gInstance.Player, __gInstance.ClosestPlayer);
                }
            }
        }

        public static class MimicButtonHandler
        {
            public static void MimicButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (!__gInstance.GlitchButton.isCoolingDown && !__gInstance.IsUsingMimic)
                {
                    __gInstance.GlitchButton.isCoolingDown = false;
                    __gInstance.GlitchButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.GlitchButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __gInstance.GlitchButton.isCoolingDown = true;
                    __gInstance.GlitchButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.GlitchButton.graphic.color = Palette.DisabledClear;
                }

                if (!__gInstance.IsUsingMimic)
                    __gInstance.GlitchButton.SetCoolDown(__gInstance.MimicTimer(), CustomGameOptions.MimicCooldown);
            }

            public static void MimicButtonPress(Glitch __gInstance, KillButton __instance)
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
                        if (rend.name == "SendButton" | rend.name == "QuickChatButton")
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

                    foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x != null && x.Data != null &&
                        x != PlayerControl.LocalPlayer && !x.Data.Disconnected))
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

        public KillButton GlitchButton
        {
            get => _glitchButton;
            set
            {
                _glitchButton = value;
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

        public void Mimic()
        {
            TimeRemaining -= Time.deltaTime;
            Utils.Morph(Player, MimicTarget);

            if (Player.Data.IsDead)
                TimeRemaining = 0f;
        }

        public void Unmimic()
        {
            MimicTarget = null;
            Utils.DefaultOutfit(Player);
            LastMimic = DateTime.UtcNow;
        }
    }
}