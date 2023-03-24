using System;
using UnityEngine;
using Object = UnityEngine.Object;
using Hazel;
using Reactor.Utilities;
using System.Linq;
using Reactor.Utilities.Extensions;
using System.Collections.Generic;
using System.Collections;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;

namespace TownOfUsReworked.PlayerLayers.Roles
{
    public class Beamer : SyndicateRole
    {
        public DateTime LastBeamed;
        public bool PressedButton;
        public bool MenuClick;
        public bool LastMouse;
        public ChatController BeamList;
        public PlayerControl BeamPlayer1;
        public PlayerControl BeamPlayer2;
        public Dictionary<byte, DateTime> UnbeamablePlayers = new();
        public AbilityButton BeamButton;

        public Beamer(PlayerControl player) : base(player)
        {
            Name = "Beamer";
            StartText = "Send A Player To Another";
            AbilitiesText = "- You can pick a player to be beamed to another player of your choice.";
            Color = CustomGameOptions.CustomSynColors ? Colors.Beamer : Colors.Syndicate;
            LastBeamed = DateTime.UtcNow;
            RoleType = RoleEnum.Beamer;
            PressedButton = false;
            MenuClick = false;
            LastMouse = false;
            BeamList = null;
            BeamPlayer1 = null;
            BeamPlayer2 = null;
            RoleAlignment = RoleAlignment.SyndicateSupport;
            AlignmentName = SSu;
            UnbeamablePlayers = new();
        }

        public float BeamTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timespan = utcNow - LastBeamed;
            var num = Utils.GetModifiedCooldown(CustomGameOptions.BeamCooldown, Utils.GetUnderdogChange(Player)) * 1000f;
            var flag2 = num - (float) timespan.TotalMilliseconds < 0f;
            return flag2 ? 0f : (num - (float) timespan.TotalMilliseconds) / 1000f;
        }

        public static IEnumerator BeamPlayers(byte player1, byte player2)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Beamer) || PlayerControl.LocalPlayer == Utils.PlayerById(player1) || PlayerControl.LocalPlayer == Utils.PlayerById(player2))
            {
                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
                } catch {}
            }

            var TP1 = Utils.PlayerById(player1);
            var TP2 = Utils.PlayerById(player2);
            var deadBodies = Object.FindObjectsOfType<DeadBody>();
            DeadBody Player1Body = null;
            DeadBody Player2Body = null;

            if (TP1.Data.IsDead)
            {
                foreach (var body in deadBodies)
                {
                    if (body.ParentId == TP1.PlayerId)
                        Player1Body = body;
                }
            }

            if (TP1.inVent && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
            {
                while (SubmergedCompatibility.GetInTransition())
                    yield return null;

                TP1.MyPhysics.ExitAllVents();
            }

            if (Player1Body == null && Player2Body == null)
            {
                TP1.MyPhysics.ResetMoveState();
                TP1.NetTransform.SnapTo(new Vector2(TP2.GetTruePosition().x, TP2.GetTruePosition().y + 0.3636f));
                TP1.MyRend().flipX = TP2.MyRend().flipX;

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body != null && Player2Body == null)
            {
                StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = TP2.GetTruePosition();

                if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(TP2.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }
            else if (Player1Body == null && Player2Body != null)
            {
                TP1.MyPhysics.ResetMoveState();
                TP1.NetTransform.SnapTo(new Vector2(Player2Body.TruePosition.x, Player2Body.TruePosition.y + 0.3636f));

                if (SubmergedCompatibility.IsSubmerged())
                {
                    if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId)
                    {
                        SubmergedCompatibility.ChangeFloor(TP1.GetTruePosition().y > -7);
                        SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                    }
                }
            }
            else if (Player1Body != null && Player2Body != null)
            {
                StopDragging(Player1Body.ParentId);
                Player1Body.transform.position = Player2Body.TruePosition;
            }

            if (PlayerControl.LocalPlayer.PlayerId == TP1.PlayerId || PlayerControl.LocalPlayer.PlayerId == TP2.PlayerId)
            {
                Coroutines.Start(Utils.FlashCoroutine(Colors.Beamer));

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            TP1.moveable = true;
            TP1.Collider.enabled = true;
            TP1.NetTransform.enabled = true;
            TP2.MyPhysics.ResetMoveState();
        }

        public static void StopDragging(byte PlayerId)
        {
            var undertakers = Role.AllRoles.Where(x => x.RoleType == RoleEnum.Undertaker && ((Undertaker)x).CurrentlyDragging != null && ((Undertaker)x).CurrentlyDragging.ParentId ==
                PlayerId);

            foreach (var undertaker in undertakers)
                ((Undertaker)undertaker).CurrentlyDragging = null;
        }

        public void BeamListUpdate(HudManager __instance)
        {
            if (PressedButton && BeamList == null && !IsBlocked)
            {
                BeamPlayer1 = null;
                BeamPlayer2 = null;
                __instance.Chat.SetVisible(false);
                BeamList = Object.Instantiate(__instance.Chat);
                BeamList.transform.SetParent(Camera.main.transform);
                BeamList.SetVisible(true);
                BeamList.Toggle();
                BeamList.TextBubble.enabled = false;
                BeamList.TextBubble.gameObject.SetActive(false);
                BeamList.TextArea.enabled = false;
                BeamList.TextArea.gameObject.SetActive(false);
                BeamList.BanButton.enabled = false;
                BeamList.BanButton.gameObject.SetActive(false);
                BeamList.CharCount.enabled = false;
                BeamList.CharCount.gameObject.SetActive(false);
                BeamList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                BeamList.OpenKeyboardButton.Destroy();
                BeamList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                BeamList.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                BeamList.BackgroundImage.enabled = false;

                foreach (var rend in BeamList.Content.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                    {
                        rend.enabled = false;
                        rend.gameObject.SetActive(false);
                    }
                }

                foreach (var bubble in BeamList.chatBubPool.activeChildren)
                {
                    bubble.enabled = false;
                    bubble.gameObject.SetActive(false);
                }

                BeamList.chatBubPool.activeChildren.Clear();

                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => (x != PlayerControl.LocalPlayer || (x == PlayerControl.LocalPlayer &&
                    CustomGameOptions.TransSelf)) && !x.Data.Disconnected))
                {
                    if (!player.Data.IsDead)
                        BeamList.AddChat(player, "Click Here");
                    else
                    {
                        foreach (var body in Object.FindObjectsOfType<DeadBody>())
                        {
                            if (body.ParentId == player.PlayerId)
                            {
                                player.Data.IsDead = false;
                                BeamList.AddChat(player, "Click Here");
                                player.Data.IsDead = true;
                            }
                        }
                    }
                }
            }

            if (BeamList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();

                if (!BeamList.IsOpen || MeetingHud.Instance || Input.GetKeyInt(KeyCode.Escape) || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    BeamList.Toggle();
                    BeamList.SetVisible(false);
                    BeamList = null;
                    PressedButton = false;
                    BeamPlayer1 = null;
                }
                else
                {
                    foreach (var bubble in BeamList.chatBubPool.activeChildren)
                    {
                        if (BeamTimer() == 0f && BeamList != null)
                        {
                            Vector2 ScreenMin = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            Vector2 ScreenMax = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);

                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x && Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y &&
                                !Input.GetMouseButtonDown(0) && LastMouse)
                            {
                                LastMouse = false;

                                foreach (var player in PlayerControl.AllPlayerControls)
                                {
                                    if (player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                                    {
                                        if (BeamPlayer1 == null)
                                        {
                                            BeamPlayer1 = player;
                                            bubble.Cast<ChatBubble>().Background.color = Colors.Beamer;
                                        }
                                        else if (player.PlayerId == BeamPlayer1.PlayerId)
                                        {
                                            BeamPlayer1 = null;
                                            bubble.Cast<ChatBubble>().Background.color = new Color32(255, 255, 255, 255);
                                        }
                                        else
                                        {
                                            PressedButton = false;
                                            BeamList.Toggle();
                                            BeamList.SetVisible(false);
                                            BeamList = null;
                                            BeamPlayer2 = player;

                                            if (!UnbeamablePlayers.ContainsKey(BeamPlayer1.PlayerId) && !UnbeamablePlayers.ContainsKey(BeamPlayer2.PlayerId))
                                            {
                                                var interaction = Utils.Interact(Player, BeamPlayer1);
                                                var interaction2 = Utils.Interact(Player, BeamPlayer2);

                                                Utils.Spread(Player, BeamPlayer1);
                                                Utils.Spread(Player, BeamPlayer2);

                                                if (BeamPlayer1.Is(RoleEnum.Pestilence) || BeamPlayer1.IsOnAlert())
                                                {
                                                    if (Player.IsShielded())
                                                    {
                                                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                                                            SendOption.Reliable);
                                                        writer2.Write(Player.GetMedic().Player.PlayerId);
                                                        writer2.Write(Player.PlayerId);
                                                        AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                        if (CustomGameOptions.ShieldBreaks)
                                                            LastBeamed = DateTime.UtcNow;

                                                        StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                        return;
                                                    }
                                                    else if (!Player.IsProtected())
                                                    {
                                                        Coroutines.Start(BeamPlayers(BeamPlayer1.PlayerId, Player.PlayerId));
                                                        var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                            SendOption.Reliable);
                                                        write2.Write((byte)ActionsRPC.Beam);
                                                        write2.Write(BeamPlayer1.PlayerId);
                                                        write2.Write(Player.PlayerId);
                                                        AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                        return;
                                                    }

                                                    LastBeamed = DateTime.UtcNow;
                                                    return;
                                                }
                                                else if (BeamPlayer2.Is(RoleEnum.Pestilence) || BeamPlayer2.IsOnAlert())
                                                {
                                                    if (Player.IsShielded())
                                                    {
                                                        var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                            SendOption.Reliable);
                                                        writer2.Write((byte)ActionsRPC.Beam);
                                                        writer2.Write(Player.GetMedic().Player.PlayerId);
                                                        writer2.Write(Player.PlayerId);
                                                        AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                        if (CustomGameOptions.ShieldBreaks)
                                                            LastBeamed = DateTime.UtcNow;

                                                        StopKill.BreakShield(Player.GetMedic().Player.PlayerId, Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                        return;
                                                    }
                                                    else if (!Player.IsProtected())
                                                    {
                                                        Coroutines.Start(BeamPlayers(BeamPlayer2.PlayerId, Player.PlayerId));
                                                        var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                            SendOption.Reliable);
                                                        write2.Write((byte)ActionsRPC.Beam);
                                                        write2.Write(BeamPlayer2.PlayerId);
                                                        write2.Write(Player.PlayerId);
                                                        AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                        return;
                                                    }

                                                    LastBeamed = DateTime.UtcNow;
                                                    return;
                                                }

                                                LastBeamed = DateTime.UtcNow;
                                                Coroutines.Start(BeamPlayers(BeamPlayer1.PlayerId, BeamPlayer2.PlayerId));
                                                var write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                                                write.Write((byte)ActionsRPC.Beam);
                                                write.Write(BeamPlayer1.PlayerId);
                                                write.Write(BeamPlayer2.PlayerId);
                                                AmongUsClient.Instance.FinishRpcImmediately(write);
                                            }
                                            else
                                                __instance.StartCoroutine(Effects.SwayX(__instance.KillButton.transform));

                                            BeamPlayer1 = null;
                                            BeamPlayer2 = null;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (!Input.GetMouseButtonDown(0) && LastMouse)
                    {
                        if (MenuClick)
                            MenuClick = false;
                        else
                        {
                            BeamList.Toggle();
                            BeamList.SetVisible(false);
                            BeamList = null;
                            PressedButton = false;
                            BeamPlayer1 = null;
                        }
                    }

                    LastMouse = Input.GetMouseButtonDown(0);
                }
            }
        }
    }
}