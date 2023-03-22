using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using System;
using Object = UnityEngine.Object;
using Hazel;
using Reactor.Utilities;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.CustomOptions;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TransporterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDTransport
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Transporter))
                return;

            var role = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);

            if (role.TransportButton == null)
                role.TransportButton = Utils.InstantiateButton();

            role.TransportButton.UpdateButton(role, "TRANSPORT", role.TransportTimer(), CustomGameOptions.TransportCooldown, AssetManager.Transport, AbilityTypes.Effect, "ActionSecondary",
                null, role.ButtonUsable, role.ButtonUsable, false, 0, 1, true, role.UsesLeft);

            if (role.PressedButton && role.TransportList == null && !role.IsBlocked)
            {
                role.TransportPlayer1 = null;
                role.TransportPlayer2 = null;
                __instance.Chat.SetVisible(false);
                role.TransportList = Object.Instantiate(__instance.Chat);
                role.TransportList.transform.SetParent(Camera.main.transform);
                role.TransportList.SetVisible(true);
                role.TransportList.Toggle();
                role.TransportList.TextBubble.enabled = false;
                role.TransportList.TextBubble.gameObject.SetActive(false);
                role.TransportList.TextArea.enabled = false;
                role.TransportList.TextArea.gameObject.SetActive(false);
                role.TransportList.BanButton.enabled = false;
                role.TransportList.BanButton.gameObject.SetActive(false);
                role.TransportList.CharCount.enabled = false;
                role.TransportList.CharCount.gameObject.SetActive(false);
                role.TransportList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                role.TransportList.OpenKeyboardButton.Destroy();
                role.TransportList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                role.TransportList.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                role.TransportList.BackgroundImage.enabled = false;

                foreach (var rend in role.TransportList.Content.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                    {
                        rend.enabled = false;
                        rend.gameObject.SetActive(false);
                    }
                }

                foreach (var bubble in role.TransportList.chatBubPool.activeChildren)
                {
                    bubble.enabled = false;
                    bubble.gameObject.SetActive(false);
                }

                role.TransportList.chatBubPool.activeChildren.Clear();

                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => (x != PlayerControl.LocalPlayer || (x == PlayerControl.LocalPlayer &&
                    CustomGameOptions.TransSelf)) && !x.Data.Disconnected))
                {
                    if (!player.Data.IsDead)
                        role.TransportList.AddChat(player, "Click Here");
                    else
                    {
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deadBodies)
                        {
                            if (body.ParentId == player.PlayerId)
                            {
                                player.Data.IsDead = false;
                                role.TransportList.AddChat(player, "Click Here");
                                player.Data.IsDead = true;
                            }
                        }
                    }
                }
            }

            if (role.TransportList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();

                if (!role.TransportList.IsOpen || MeetingHud.Instance || Input.GetKeyInt(KeyCode.Escape) || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    role.TransportList.Toggle();
                    role.TransportList.SetVisible(false);
                    role.TransportList = null;
                    role.PressedButton = false;
                    role.TransportPlayer1 = null;
                }
                else
                {
                    foreach (var bubble in role.TransportList.chatBubPool.activeChildren)
                    {
                        if (role.TransportTimer() == 0f && role.TransportList != null)
                        {
                            var ScreenMin = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            var ScreenMax = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);

                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x && Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y)
                            {
                                if (!Input.GetMouseButtonDown(0) && role.LastMouse)
                                {
                                    role.LastMouse = false;

                                    foreach (var player in PlayerControl.AllPlayerControls)
                                    {
                                        if (player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                                        {
                                            if (role.TransportPlayer1 == null)
                                            {
                                                role.TransportPlayer1 = player;
                                                bubble.Cast<ChatBubble>().Background.color = Colors.Transporter;
                                            }
                                            else if (player.PlayerId == role.TransportPlayer1.PlayerId)
                                            {
                                                role.TransportPlayer1 = null;
                                                bubble.Cast<ChatBubble>().Background.color = new Color32(255, 255, 255, 255);
                                            }
                                            else
                                            {
                                                role.PressedButton = false;
                                                role.TransportList.Toggle();
                                                role.TransportList.SetVisible(false);
                                                role.TransportList = null;
                                                role.TransportPlayer2 = player;

                                                if (!role.UntransportablePlayers.ContainsKey(role.TransportPlayer1.PlayerId) &&
                                                    !role.UntransportablePlayers.ContainsKey(role.TransportPlayer2.PlayerId))
                                                {
                                                    var interaction = Utils.Interact(role.Player, role.TransportPlayer1);
                                                    var interaction2 = Utils.Interact(role.Player, role.TransportPlayer2);

                                                    Utils.Spread(role.Player, role.TransportPlayer1);
                                                    Utils.Spread(role.Player, role.TransportPlayer2);

                                                    if (role.TransportPlayer1.Is(RoleEnum.Pestilence) || role.TransportPlayer1.IsOnAlert())
                                                    {
                                                        if (role.Player.IsShielded())
                                                        {
                                                            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                                                                SendOption.Reliable);
                                                            writer2.Write(role.Player.GetMedic().Player.PlayerId);
                                                            writer2.Write(role.Player.PlayerId);
                                                            AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                            if (CustomGameOptions.ShieldBreaks)
                                                                role.LastTransported = DateTime.UtcNow;

                                                            StopKill.BreakShield(role.Player.GetMedic().Player.PlayerId, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                            return;
                                                        }
                                                        else if (!role.Player.IsProtected())
                                                        {
                                                            Coroutines.Start(Transporter.TransportPlayers(role.TransportPlayer1.PlayerId, role.Player.PlayerId, true));
                                                            var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                SendOption.Reliable);
                                                            write2.Write((byte)ActionsRPC.Transport);
                                                            write2.Write(role.TransportPlayer1.PlayerId);
                                                            write2.Write(role.Player.PlayerId);
                                                            write2.Write(true);
                                                            AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                            return;
                                                        }

                                                        role.LastTransported = DateTime.UtcNow;
                                                        return;
                                                    }
                                                    else if (role.TransportPlayer2.Is(RoleEnum.Pestilence) || role.TransportPlayer2.IsOnAlert())
                                                    {
                                                        if (role.Player.IsShielded())
                                                        {
                                                            var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                SendOption.Reliable);
                                                            writer2.Write((byte)ActionsRPC.Transport);
                                                            writer2.Write(role.Player.GetMedic().Player.PlayerId);
                                                            writer2.Write(role.Player.PlayerId);
                                                            AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                            if (CustomGameOptions.ShieldBreaks)
                                                                role.LastTransported = DateTime.UtcNow;

                                                            StopKill.BreakShield(role.Player.GetMedic().Player.PlayerId, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                            return;
                                                        }
                                                        else if (!role.Player.IsProtected())
                                                        {
                                                            Coroutines.Start(Transporter.TransportPlayers(role.TransportPlayer2.PlayerId, role.Player.PlayerId, true));
                                                            var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                SendOption.Reliable);
                                                            write2.Write((byte)ActionsRPC.Transport);
                                                            write2.Write(role.TransportPlayer2.PlayerId);
                                                            write2.Write(role.Player.PlayerId);
                                                            write2.Write(true);
                                                            AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                            return;
                                                        }

                                                        role.LastTransported = DateTime.UtcNow;
                                                        return;
                                                    }

                                                    role.LastTransported = DateTime.UtcNow;
                                                    role.UsesLeft--;
                                                    Coroutines.Start(Transporter.TransportPlayers(role.TransportPlayer1.PlayerId, role.TransportPlayer2.PlayerId, false));
                                                    var write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                                                    write.Write((byte)ActionsRPC.Transport);
                                                    write.Write(role.TransportPlayer1.PlayerId);
                                                    write.Write(role.TransportPlayer2.PlayerId);
                                                    write.Write(false);
                                                    AmongUsClient.Instance.FinishRpcImmediately(write);
                                                }
                                                else
                                                    (__instance as MonoBehaviour).StartCoroutine(Effects.SwayX(__instance.KillButton.transform));

                                                role.TransportPlayer1 = null;
                                                role.TransportPlayer2 = null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (!Input.GetMouseButtonDown(0) && role.LastMouse)
                {
                    if (role.MenuClick)
                        role.MenuClick = false;
                    else
                    {
                        role.TransportList.Toggle();
                        role.TransportList.SetVisible(false);
                        role.TransportList = null;
                        role.PressedButton = false;
                        role.TransportPlayer1 = null;
                    }
                }

                role.LastMouse = Input.GetMouseButtonDown(0);
            }
        }
    }
}