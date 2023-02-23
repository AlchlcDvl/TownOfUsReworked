using Hazel;
using System;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using TownOfUsReworked.PlayerLayers.Modifiers;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.NeutralsMod;
using System.Collections;
using Reactor.Utilities;
//using TownOfUsReworked.Patches;

namespace TownOfUsReworked.PlayerLayers.Roles
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
        public bool IsUsingMimic;
        public bool IsUsingHack;
        public bool MimicEnabled;
        public PlayerControl MimicTarget { get; set; }
        public bool GlitchWins { get; set; }
        public static List<byte> HackedPlayers = new List<byte>();
        public static Dictionary<byte, float> HackedKnows = new Dictionary<byte, float>();
        public bool LastMouse;
        public static Sprite MimicSprite = TownOfUsReworked.MimicSprite;
        public static Sprite HackSprite = TownOfUsReworked.HackSprite;
        public static Sprite EraseData = TownOfUsReworked.EraseDataSprite;
        public static Sprite LockSprite = TownOfUsReworked.Lock;

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
            AlignmentName = NK;
            //IntroSound = TownOfUsReworked.GlitchIntro;
            RoleDescription = "You are a Glitch! You are an otherworldly being who only seeks destruction. Mess with the player's systems so that they are " +
                "unable to oppose you and mimic others to frame them! Do not let anyone live.";
        }

        internal override bool GameEnd(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected)
                return true;

            if (IsRecruit && Utils.CabalWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CabalWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.AllNeutralsWin() && NotDefective)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.AllNeutralsWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.AllNKsWin() && NotDefective)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.AllNKsWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsCrewAlly && Utils.CrewWins())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.CrewWin);
                writer.Write(Player.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsIntAlly && Utils.IntrudersWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.IntruderWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsSynAlly && Utils.SyndicateWins())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsPersuaded && Utils.SectWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SectWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsBitten && Utils.UndeadWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.UndeadWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (IsResurrected && Utils.ReanimatedWin())
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.ReanimatedWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.NKWins(RoleType) && NotDefective)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                writer.Write((byte)WinLoseRPC.SyndicateWin);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.EndGame();
                return false;
            }
            else if (Utils.NKWins(RoleType) && NotDefective)
            {
                Wins();
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
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

        public override void IntroPrefix(IntroCutscene._ShowTeam_d__32 __instance)
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
            else if (IsIntAlly || IsSynAlly)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Is(Faction) && player != PlayerControl.LocalPlayer)
                        team.Add(player);
                }
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

        /*public static void SetHackedKnows(bool active = true)
        {
            if (active)
            {
                byte localPlayerId = PlayerControl.LocalPlayer.PlayerId;
                HackedKnows.Add(localPlayerId, CustomGameOptions.HackDuration);
                HackedPlayers.RemoveAll(x => x == localPlayerId);
            }

            HudManagerStartPatch.SetAllButtonsHackedStatus(active);
        }*/

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
            if (Player.Data.IsDead)
                return;

            KillButtonHandler.KillButtonUpdate(this, __instance);
            MimicButtonHandler.MimicButtonUpdate(this, __instance);
            HackButtonHandler.HackButtonUpdate(this, __instance);

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
                                        RpcSetMimicked(PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text).FirstOrDefault());
                                        break;
                                    }

                                    LastMouse = Input.GetMouseButtonDown(0);
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool UseAbility(KillButton __instance)
        {
            if (!Utils.ButtonUsable(__instance))
                return false;

            if (__instance == HackButton)
                HackButtonHandler.HackButtonPress(this, __instance);
            else if (__instance == MimicButton)
                MimicButtonHandler.MimicButtonPress(this, __instance);
            else if (__instance == KillButton)
                KillButtonHandler.KillButtonPress(this, __instance);

            return false;
        }

        public void RpcSetHacked(PlayerControl hacked)
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.GlitchRoleblock);
            writer.Write(hacked.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            SetHacked(hacked);
        }

        public void SetHacked(PlayerControl hacked)
        {
            LastHack = DateTime.UtcNow;
            Coroutines.Start(AbilityCoroutine.Hack(this, hacked));
        }

        public void RpcSetMimicked(PlayerControl mimicked)
        {
            Coroutines.Start(AbilityCoroutine.Mimic(this, mimicked));
        }

        public static class AbilityCoroutine
        {
            public static Dictionary<byte, DateTime> tickDictionary = new Dictionary<byte, DateTime>();

            public static IEnumerator Hack(Glitch __instance, PlayerControl hackPlayer)
            {
                GameObject[] lockImg = { null, null, null, null, null, null, null, null, null };

                if (tickDictionary.ContainsKey(hackPlayer.PlayerId))
                {
                    tickDictionary[hackPlayer.PlayerId] = DateTime.UtcNow;
                    yield break;
                }

                while (true)
                {
                    __instance.IsUsingHack = true;

                    if (PlayerControl.LocalPlayer == hackPlayer)
                    {
                        if (HudManager.Instance.KillButton != null)
                        {
                            if (lockImg[0] == null)
                            {
                                lockImg[0] = new GameObject();
                                var lockImgR = lockImg[0].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[0].layer = 5;
                            lockImg[0].transform.position = new Vector3(HudManager.Instance.KillButton.transform.position.x, HudManager.Instance.KillButton.transform.position.y, -50f);
                            HudManager.Instance.KillButton.enabled = false;
                            HudManager.Instance.KillButton.graphic.color = Palette.DisabledClear;
                            HudManager.Instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                        }

                        if (HudManager.Instance.UseButton != null || HudManager.Instance.PetButton != null)
                        {
                            if (lockImg[1] == null)
                            {
                                lockImg[1] = new GameObject();
                                var lockImgR = lockImg[1].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            if (HudManager.Instance.UseButton != null)
                            {
                                lockImg[1].transform.position = new Vector3(HudManager.Instance.UseButton.transform.position.x, HudManager.Instance.UseButton.transform.position.y, -50f);
                                lockImg[1].layer = 5;
                                HudManager.Instance.UseButton.enabled = false;
                                HudManager.Instance.UseButton.graphic.color = Palette.DisabledClear;
                                HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 1f);
                            }
                            else
                            {
                                lockImg[1].transform.position = new Vector3(HudManager.Instance.PetButton.transform.position.x, HudManager.Instance.PetButton.transform.position.y, -50f);
                                lockImg[1].layer = 5;
                                HudManager.Instance.PetButton.enabled = false;
                                HudManager.Instance.PetButton.graphic.color = Palette.DisabledClear;
                                HudManager.Instance.PetButton.graphic.material.SetFloat("_Desat", 1f);
                            }
                        }

                        if (HudManager.Instance.ReportButton != null)
                        {
                            if (lockImg[2] == null)
                            {
                                lockImg[2] = new GameObject();
                                var lockImgR = lockImg[2].AddComponent<SpriteRenderer>();
                                lockImgR.sprite = LockSprite;
                            }

                            lockImg[2].transform.position = new Vector3(HudManager.Instance.ReportButton.transform.position.x, HudManager.Instance.ReportButton.transform.position.y, -50f);
                            lockImg[2].layer = 5;
                            HudManager.Instance.ReportButton.enabled = false;
                            HudManager.Instance.ReportButton.SetActive(false);
                        }

                        var role = GetRole(PlayerControl.LocalPlayer);

                        if (role != null)
                        {
                            if (role.AbilityButtons.Count > 0)
                            {
                                int i = 3;

                                foreach (var button in role.AbilityButtons)
                                {
                                    if (lockImg[i] == null)
                                    {
                                        lockImg[i] = new GameObject();
                                        var lockImgR = lockImg[i].AddComponent<SpriteRenderer>();
                                        lockImgR.sprite = LockSprite;
                                    }

                                    lockImg[i].transform.position = new Vector3(button.transform.position.x, button.transform.position.y, -50f);
                                    lockImg[i].layer = 5;
                                    button.enabled = false;
                                    button.graphic.color = Palette.DisabledClear;
                                    button.graphic.material.SetFloat("_Desat", 1f);
                                    i++;
                                }
                            }
                        }

                        if (Minigame.Instance)
                            Minigame.Instance.Close();

                        if (MapBehaviour.Instance)
                            MapBehaviour.Instance.Close();
                    }

                    var totalHacktime = (DateTime.UtcNow - tickDictionary[hackPlayer.PlayerId]).TotalMilliseconds / 1000;

                    if (MeetingHud.Instance || totalHacktime > CustomGameOptions.HackDuration || hackPlayer == null || hackPlayer.Data.IsDead)
                    {
                        __instance.IsUsingHack = false;

                        foreach (var obj in lockImg)
                        {
                            if (obj != null)
                                obj.SetActive(false);
                        }

                        if (PlayerControl.LocalPlayer == hackPlayer)
                        {
                            if (HudManager.Instance.UseButton != null)
                            {
                                HudManager.Instance.UseButton.enabled = true;
                                HudManager.Instance.UseButton.graphic.color = Palette.EnabledColor;
                                HudManager.Instance.UseButton.graphic.material.SetFloat("_Desat", 0f);
                            }
                            else
                            {
                                HudManager.Instance.PetButton.enabled = true;
                                HudManager.Instance.PetButton.graphic.color = Palette.EnabledColor;
                                HudManager.Instance.PetButton.graphic.material.SetFloat("_Desat", 0f);
                            }

                            HudManager.Instance.ReportButton.enabled = true;
                            HudManager.Instance.KillButton.enabled = true;
                            var role = GetRole(PlayerControl.LocalPlayer);

                            if (role != null)
                            {
                                if (role.AbilityButtons.Count > 0)
                                {
                                    foreach (var button in role.AbilityButtons)
                                    {
                                        button.enabled = true;
                                        button.graphic.color = Palette.EnabledColor;
                                        button.graphic.material.SetFloat("_Desat", 0f);
                                    }
                                }
                            }
                        }

                        tickDictionary.Remove(hackPlayer.PlayerId);
                        yield break;
                    }

                    yield return null;
                }
            }

            public static IEnumerator Mimic(Glitch __instance, PlayerControl mimicPlayer)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                writer.Write((byte)ActionsRPC.SetMimic);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(mimicPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                Utils.Morph(__instance.Player, mimicPlayer);
                var mimicActivation = DateTime.UtcNow;

                while (true)
                {
                    __instance.MimicTarget = mimicPlayer;
                    var totalMimickTime = (DateTime.UtcNow - mimicActivation).TotalMilliseconds / 1000;

                    if (__instance.Player.Data.IsDead)
                        totalMimickTime = CustomGameOptions.MimicDuration;

                    if (totalMimickTime >= CustomGameOptions.MimicDuration || PlayerControl.LocalPlayer.Data.IsDead || Classes.GameStates.IsEnded)
                    {
                        __instance.LastMimic = DateTime.UtcNow;
                        __instance.MimicTarget = null;
                        Utils.DefaultOutfit(__instance.Player);

                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer2.Write((byte)ActionsRPC.RpcResetAnim);
                        writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                        writer2.Write(mimicPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer2);
                        yield break;
                    }

                    Utils.Morph(__instance.Player, mimicPlayer);
                    __instance.MimicButton.SetCoolDown(CustomGameOptions.MimicDuration - (float)totalMimickTime, CustomGameOptions.MimicDuration);
                    yield return null;
                }
            }
        }

        public static class KillButtonHandler
        {
            public static void KillButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (__gInstance.KillButton == null)
                {
                    __gInstance.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.KillButton.graphic.sprite = EraseData;
                    __gInstance.KillButton.graphic.enabled = true;
                    __gInstance.KillButton.gameObject.SetActive(false);
                }

                __instance.KillButton.gameObject.SetActive(Utils.SetActive(__gInstance.Player, __instance));
                __instance.KillButton.SetCoolDown(CustomGameOptions.GlitchKillCooldown - (float)(DateTime.UtcNow - __gInstance.LastKilled).TotalSeconds, CustomGameOptions.GlitchKillCooldown);
                Utils.SetTarget(ref __gInstance.ClosestPlayer, __gInstance.KillButton);

                if (Utils.EnableAbilityButton(__gInstance.KillButton, __gInstance.Player, __gInstance.ClosestPlayer))
                {
                    __gInstance.KillButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.KillButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __gInstance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.KillButton.graphic.color = Palette.DisabledClear;
                }
            }

            public static void KillButtonPress(Glitch __gInstance, KillButton __instance)
            {
                if (__gInstance.KillTimer() != 0f)
                    return;

                if (Utils.IsTooFar(__gInstance.Player, __gInstance.ClosestPlayer))
                    return;

                var interact = Utils.Interact(__gInstance.Player, __gInstance.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true || interact[0] == true)
                    __gInstance.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    __gInstance.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    __gInstance.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);
            }
        }

        public static class HackButtonHandler
        {
            public static void HackButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (__gInstance.HackButton == null)
                {
                    __gInstance.HackButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.HackButton.graphic.sprite = HackSprite;
                    __gInstance.HackButton.graphic.enabled = true;
                    __gInstance.HackButton.gameObject.SetActive(false);
                }

                __gInstance.HackButton.gameObject.SetActive(Utils.SetActive(__gInstance.Player, __instance));
                __gInstance.HackButton.SetCoolDown(CustomGameOptions.HackCooldown - (float)(DateTime.UtcNow - __gInstance.LastHack).TotalSeconds, CustomGameOptions.HackCooldown);
                Utils.SetTarget(ref __gInstance.ClosestPlayer, __gInstance.HackButton);

                if (Utils.EnableAbilityButton(__gInstance.HackButton, __gInstance.Player, __gInstance.ClosestPlayer, __gInstance.IsUsingHack))
                {
                    __gInstance.HackButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.HackButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __gInstance.HackButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.HackButton.graphic.color = Palette.DisabledClear;
                }
            }

            public static void HackButtonPress(Glitch __gInstance, KillButton __instance)
            {
                if (__gInstance.HackTimer() != 0f)
                    return;

                if (Utils.IsTooFar(__gInstance.Player, __gInstance.ClosestPlayer))
                    return;
                
                if (__gInstance.IsUsingHack)
                    return;

                if (__gInstance.HackTarget != null)
                {
                    var interact = Utils.Interact(__gInstance.Player, __gInstance.HackTarget, GetRoleValue(RoleEnum.Pestilence), false, false, GetRoleValue(RoleEnum.SerialKiller));

                    if (interact[3] == true)
                        __gInstance.RpcSetHacked(__gInstance.HackTarget);

                    if (interact[0] == true)
                        __gInstance.LastHack = DateTime.UtcNow;
                    else if (interact[1] == true)
                        __gInstance.LastHack = DateTime.UtcNow.AddSeconds(CustomGameOptions.ProtectKCReset);

                    return;
                }
            }
        }

        public static class MimicButtonHandler
        {
            public static void MimicButtonUpdate(Glitch __gInstance, HudManager __instance)
            {
                if (__gInstance.MimicButton == null)
                {
                    __gInstance.MimicButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    __gInstance.MimicButton.graphic.sprite = MimicSprite;
                    __gInstance.MimicButton.gameObject.SetActive(false);
                    __gInstance.MimicButton.graphic.enabled = true;
                }

                __gInstance.MimicButton.gameObject.SetActive(Utils.SetActive(__gInstance.Player, __instance));

                if (!__gInstance.IsUsingMimic)
                    __gInstance.MimicButton.SetCoolDown(CustomGameOptions.MimicCooldown - (float)(DateTime.UtcNow - __gInstance.LastMimic).TotalSeconds, CustomGameOptions.MimicCooldown);

                if (Utils.EnableAbilityButton(__gInstance.MimicButton, __gInstance.Player, null, __gInstance.IsUsingMimic))
                {
                    __gInstance.MimicButton.graphic.material.SetFloat("_Desat", 0f);
                    __gInstance.MimicButton.graphic.color = Palette.EnabledColor;
                }
                else
                {
                    __gInstance.MimicButton.graphic.material.SetFloat("_Desat", 1f);
                    __gInstance.MimicButton.graphic.color = Palette.DisabledClear;
                }
            }

            public static void MimicButtonPress(Glitch __gInstance, KillButton __instance)
            {
                if (__gInstance.MimicTimer() != 0f)
                    return;

                if (__gInstance.IsUsingMimic)
                    return;

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
}