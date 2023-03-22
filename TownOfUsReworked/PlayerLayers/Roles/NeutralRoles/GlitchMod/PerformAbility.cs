using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.CustomOptions;
using System;
using Hazel;
using Object = UnityEngine.Object;
using Reactor.Utilities.Extensions;
using UnityEngine;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GlitchMod
{
    [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(AbilityButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Glitch))
                return true;

            var role = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);

            if (__instance == role.HackButton)
            {
                if (role.HackTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer);

                if (interact[3] == true)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.GlitchRoleblock);
                    writer.Write(PlayerControl.LocalPlayer.PlayerId);
                    writer.Write(role.ClosestPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    role.TimeRemaining = CustomGameOptions.HackDuration;
                    role.HackTarget = role.ClosestPlayer;
                    var targetRole = Role.GetRole(role.HackTarget);
                    targetRole.IsBlocked = !targetRole.RoleBlockImmune;
                    role.Hack();
                }
                else if (interact[0] == true)
                    role.LastHack = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastHack.AddSeconds(CustomGameOptions.ProtectKCReset);

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;

                if (role.KillTimer() != 0f)
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, true);

                if (interact[3] == true || interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }
            else if (__instance == role.MimicButton)
            {
                if (role.MimicTimer() != 0f)
                    return false;

                if (role.MimicList == null)
                {
                    role.MimicTarget = null;
                    HudManager.Instance.Chat.SetVisible(false);
                    role.MimicList = Object.Instantiate(HudManager.Instance.Chat);
                    role.MimicList.transform.SetParent(Camera.main.transform);
                    role.MimicList.SetVisible(true);
                    role.MimicList.Toggle();
                    role.MimicList.TextBubble.enabled = false;
                    role.MimicList.TextBubble.gameObject.SetActive(false);
                    role.MimicList.TextArea.enabled = false;
                    role.MimicList.TextArea.gameObject.SetActive(false);
                    role.MimicList.BanButton.enabled = false;
                    role.MimicList.BanButton.gameObject.SetActive(false);
                    role.MimicList.CharCount.enabled = false;
                    role.MimicList.CharCount.gameObject.SetActive(false);
                    role.MimicList.OpenKeyboardButton.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    role.MimicList.OpenKeyboardButton.Destroy();
                    role.MimicList.gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    role.MimicList.gameObject.transform.GetChild(0).gameObject.SetActive(false);
                    role.MimicList.BackgroundImage.enabled = false;

                    foreach (var rend in role.MimicList.Content.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (rend.name == "SendButton" || rend.name == "QuickChatButton")
                        {
                            rend.enabled = false;
                            rend.gameObject.SetActive(false);
                        }
                    }

                    foreach (var bubble in role.MimicList.chatBubPool.activeChildren)
                    {
                        bubble.enabled = false;
                        bubble.gameObject.SetActive(false);
                    }

                    role.MimicList.chatBubPool.activeChildren.Clear();

                    foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x != null && x.Data != null && x != PlayerControl.LocalPlayer && !x.Data.Disconnected))
                    {
                        if (!player.Data.IsDead)
                            role.MimicList.AddChat(player, "Click Here");
                        else
                        {
                            var deadBodies = Object.FindObjectsOfType<DeadBody>();

                            foreach (var body in deadBodies)
                            {
                                if (body.ParentId == player.PlayerId)
                                {
                                    player.Data.IsDead = false;
                                    role.MimicList.AddChat(player, "Click Here");
                                    player.Data.IsDead = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    role.MimicList.Toggle();
                    role.MimicList.SetVisible(false);
                    role.MimicList = null;
                }

                return false;
            }
            
            return true;
        }
    }
}