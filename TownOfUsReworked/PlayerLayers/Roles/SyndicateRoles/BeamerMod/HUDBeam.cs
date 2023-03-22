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

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BeamerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBeam
    {
        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Beamer))
                return;

            var role = Role.GetRole<Beamer>(PlayerControl.LocalPlayer);

            if (role.BeamButton == null)
                role.BeamButton = Utils.InstantiateButton();

            role.BeamButton.UpdateButton(role, "BEAM", role.BeamTimer(), CustomGameOptions.BeamCooldown, AssetManager.Placeholder, AbilityTypes.Effect, "Secondary");

            if (role.PressedButton && role.BeamList == null && !role.IsBlocked)
            {
                role.BeamPlayer1 = null;
                role.BeamPlayer2 = null;
                __instance.Chat.SetVisible(false);
                role.BeamList = Object.Instantiate(__instance.Chat);
                role.BeamList.transform.SetParent(Camera.main.transform);
                role.BeamList.SetVisible(true);
                role.BeamList.Toggle();
                role.BeamList.TextBubble.enabled = false;
                role.BeamList.TextBubble.gameObject.SetActive(false);
                role.BeamList.TextArea.enabled = false;
                role.BeamList.TextArea.gameObject.SetActive(false);
                role.BeamList.BanButton.enabled = false;
                role.BeamList.BanButton.gameObject.SetActive(false);
                role.BeamList.CharCount.enabled = false;
                role.BeamList.CharCount.gameObject.SetActive(false);
                role.BeamList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                role.BeamList.OpenKeyboardButton.Destroy();
                role.BeamList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                role.BeamList.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                role.BeamList.BackgroundImage.enabled = false;

                foreach (var rend in role.BeamList.Content.GetComponentsInChildren<SpriteRenderer>())
                {
                    if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                    {
                        rend.enabled = false;
                        rend.gameObject.SetActive(false);
                    }
                }

                foreach (var bubble in role.BeamList.chatBubPool.activeChildren)
                {
                    bubble.enabled = false;
                    bubble.gameObject.SetActive(false);
                }

                role.BeamList.chatBubPool.activeChildren.Clear();

                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => (x != PlayerControl.LocalPlayer || (x == PlayerControl.LocalPlayer &&
                    CustomGameOptions.TransSelf)) && !x.Data.Disconnected))
                {
                    if (!player.Data.IsDead)
                        role.BeamList.AddChat(player, "Click Here");
                    else
                    {
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deadBodies)
                        {
                            if (body.ParentId == player.PlayerId)
                            {
                                player.Data.IsDead = false;
                                role.BeamList.AddChat(player, "Click Here");
                                player.Data.IsDead = true;
                            }
                        }
                    }
                }
            }

            if (role.BeamList != null)
            {
                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();

                if (!role.BeamList.IsOpen || MeetingHud.Instance || Input.GetKeyInt(KeyCode.Escape) || PlayerControl.LocalPlayer.Data.IsDead)
                {
                    role.BeamList.Toggle();
                    role.BeamList.SetVisible(false);
                    role.BeamList = null;
                    role.PressedButton = false;
                    role.BeamPlayer1 = null;
                }
                else
                {
                    foreach (var bubble in role.BeamList.chatBubPool.activeChildren)
                    {
                        if (role.BeamTimer() == 0f && role.BeamList != null)
                        {
                            Vector2 ScreenMin = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.min);
                            Vector2 ScreenMax = Camera.main.WorldToScreenPoint(bubble.Cast<ChatBubble>().Background.bounds.max);

                            if (Input.mousePosition.x > ScreenMin.x && Input.mousePosition.x < ScreenMax.x)
                            {
                                if (Input.mousePosition.y > ScreenMin.y && Input.mousePosition.y < ScreenMax.y)
                                {
                                    if (!Input.GetMouseButtonDown(0) && role.LastMouse)
                                    {
                                        role.LastMouse = false;

                                        foreach (var player in PlayerControl.AllPlayerControls)
                                        {
                                            if (player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                                            {
                                                if (role.BeamPlayer1 == null)
                                                {
                                                    role.BeamPlayer1 = player;
                                                    bubble.Cast<ChatBubble>().Background.color = Colors.Beamer;
                                                }
                                                else if (player.PlayerId == role.BeamPlayer1.PlayerId)
                                                {
                                                    role.BeamPlayer1 = null;
                                                    bubble.Cast<ChatBubble>().Background.color = new Color32(255, 255, 255, 255);
                                                }
                                                else
                                                {
                                                    role.PressedButton = false;
                                                    role.BeamList.Toggle();
                                                    role.BeamList.SetVisible(false);
                                                    role.BeamList = null;
                                                    role.BeamPlayer2 = player;

                                                    if (!role.UnbeamablePlayers.ContainsKey(role.BeamPlayer1.PlayerId) && !role.UnbeamablePlayers.ContainsKey(role.BeamPlayer2.PlayerId))
                                                    {
                                                        var interaction = Utils.Interact(role.Player, role.BeamPlayer1);
                                                        var interaction2 = Utils.Interact(role.Player, role.BeamPlayer2);

                                                        Utils.Spread(role.Player, role.BeamPlayer1);
                                                        Utils.Spread(role.Player, role.BeamPlayer2);

                                                        if (role.BeamPlayer1.Is(RoleEnum.Pestilence) || role.BeamPlayer1.IsOnAlert())
                                                        {
                                                            if (role.Player.IsShielded())
                                                            {
                                                                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                                                                    SendOption.Reliable);
                                                                writer2.Write(role.Player.GetMedic().Player.PlayerId);
                                                                writer2.Write(role.Player.PlayerId);
                                                                AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                                if (CustomGameOptions.ShieldBreaks)
                                                                    role.LastBeamed = DateTime.UtcNow;

                                                                StopKill.BreakShield(role.Player.GetMedic().Player.PlayerId, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                                return;
                                                            }
                                                            else if (!role.Player.IsProtected())
                                                            {
                                                                Coroutines.Start(Beamer.BeamPlayers(role.BeamPlayer1.PlayerId, role.Player.PlayerId, true));
                                                                var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                    SendOption.Reliable);
                                                                write2.Write((byte)ActionsRPC.Beam);
                                                                write2.Write(role.BeamPlayer1.PlayerId);
                                                                write2.Write(role.Player.PlayerId);
                                                                write2.Write(true);
                                                                AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                                return;
                                                            }

                                                            role.LastBeamed = DateTime.UtcNow;
                                                            return;
                                                        }
                                                        else if (role.BeamPlayer2.Is(RoleEnum.Pestilence) || role.BeamPlayer2.IsOnAlert())
                                                        {
                                                            if (role.Player.IsShielded())
                                                            {
                                                                var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                    SendOption.Reliable);
                                                                writer2.Write((byte)ActionsRPC.Beam);
                                                                writer2.Write(role.Player.GetMedic().Player.PlayerId);
                                                                writer2.Write(role.Player.PlayerId);
                                                                AmongUsClient.Instance.FinishRpcImmediately(writer2);

                                                                if (CustomGameOptions.ShieldBreaks)
                                                                    role.LastBeamed = DateTime.UtcNow;

                                                                StopKill.BreakShield(role.Player.GetMedic().Player.PlayerId, role.Player.PlayerId, CustomGameOptions.ShieldBreaks);
                                                                return;
                                                            }
                                                            else if (!role.Player.IsProtected())
                                                            {
                                                                Coroutines.Start(Beamer.BeamPlayers(role.BeamPlayer2.PlayerId, role.Player.PlayerId, true));
                                                                var write2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action,
                                                                    SendOption.Reliable);
                                                                write2.Write((byte)ActionsRPC.Beam);
                                                                write2.Write(role.BeamPlayer2.PlayerId);
                                                                write2.Write(role.Player.PlayerId);
                                                                write2.Write(true);
                                                                AmongUsClient.Instance.FinishRpcImmediately(write2);
                                                                return;
                                                            }

                                                            role.LastBeamed = DateTime.UtcNow;
                                                            return;
                                                        }

                                                        role.LastBeamed = DateTime.UtcNow;
                                                        Coroutines.Start(Beamer.BeamPlayers(role.BeamPlayer1.PlayerId, role.BeamPlayer2.PlayerId, false));
                                                        var write = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                                                        write.Write((byte)ActionsRPC.Beam);
                                                        write.Write(role.BeamPlayer1.PlayerId);
                                                        write.Write(role.BeamPlayer2.PlayerId);
                                                        write.Write(false);
                                                        AmongUsClient.Instance.FinishRpcImmediately(write);
                                                    }
                                                    else
                                                        (__instance as MonoBehaviour).StartCoroutine(Effects.SwayX(__instance.KillButton.transform));

                                                    role.BeamPlayer1 = null;
                                                    role.BeamPlayer2 = null;
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
                            role.BeamList.Toggle();
                            role.BeamList.SetVisible(false);
                            role.BeamList = null;
                            role.PressedButton = false;
                            role.BeamPlayer1 = null;
                        }
                    }

                    role.LastMouse = Input.GetMouseButtonDown(0);
                }
            }
        }
    }
}