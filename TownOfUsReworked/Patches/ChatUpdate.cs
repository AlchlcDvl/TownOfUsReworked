using HarmonyLib;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.CustomOptions;
using UnityEngine;
using TownOfUsReworked.Data;
using TownOfUsReworked.Extensions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class ChatUpdate
    {
        private static float _time;

        public static void Postfix(HudManager __instance)
        {
            if (__instance.Chat)
            {
                if (_time >= CustomGameOptions.ChatCooldown)
                {
                    __instance.Chat.TimeSinceLastMessage = 3f;
                    _time -= CustomGameOptions.ChatCooldown;
                }
                else
                    _time += Time.deltaTime;

                foreach (var bubble in __instance.Chat.chatBubPool.activeChildren)
                {
                    if (bubble.Cast<ChatBubble>().NameText != null)
                    {
                        bubble.Cast<ChatBubble>().NameText.alignment = TMPro.TextAlignmentOptions.Left;
                        bubble.Cast<ChatBubble>().TextArea.alignment = TMPro.TextAlignmentOptions.TopLeft;
                    }
                }

                if (ConstantVariables.IsLobby)
                    return;

                foreach (var bubble in __instance.Chat.chatBubPool.activeChildren)
                {
                    if (bubble.Cast<ChatBubble>().NameText != null)
                    {
                        if (bubble.Cast<ChatBubble>().NameText.text.Contains(PlayerControl.LocalPlayer.Data.PlayerName))
                        {
                            if (Role.LocalRole != null)
                                bubble.Cast<ChatBubble>().NameText.color = Role.LocalRole.Color;

                            if (CustomGameOptions.Whispers && !bubble.Cast<ChatBubble>().NameText.text.Contains($"[{PlayerControl.LocalPlayer.PlayerId}] "))
                                bubble.Cast<ChatBubble>().NameText.text = $"[{PlayerControl.LocalPlayer.PlayerId}] " + bubble.Cast<ChatBubble>().NameText.text;
                        }
                        else
                        {
                            foreach (var player in PlayerControl.AllPlayerControls)
                            {
                                if (bubble.Cast<ChatBubble>().NameText.text.Contains(player.Data.PlayerName))
                                {
                                    var role = Role.GetRole(player);

                                    if (role != null)
                                    {
                                        if (((PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && !player.Is(Faction.Crew) && !player.Is(Faction.Neutral)) ||
                                            (PlayerControl.LocalPlayer.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None))) && CustomGameOptions.FactionSeeRoles)
                                        {
                                            bubble.Cast<ChatBubble>().NameText.color = role.Color;
                                        }
                                        else if (PlayerControl.LocalPlayer.GetFaction() == player.GetFaction() && !player.Is(Faction.Crew) && !player.Is(Faction.Neutral) &&
                                            !CustomGameOptions.FactionSeeRoles)
                                        {
                                            bubble.Cast<ChatBubble>().NameText.color = role.FactionColor;
                                        }
                                        else if (PlayerControl.LocalPlayer.GetSubFaction() == player.GetSubFaction() && !player.Is(SubFaction.None) && !CustomGameOptions.FactionSeeRoles)
                                            bubble.Cast<ChatBubble>().NameText.color = role.SubFactionColor;
                                        else
                                            bubble.Cast<ChatBubble>().NameText.color = Color.white;
                                    }

                                    if (CustomGameOptions.Whispers && !bubble.Cast<ChatBubble>().NameText.text.Contains($"[{player.PlayerId}] "))
                                        bubble.Cast<ChatBubble>().NameText.text = $"[{player.PlayerId}] " + bubble.Cast<ChatBubble>().NameText.text;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}