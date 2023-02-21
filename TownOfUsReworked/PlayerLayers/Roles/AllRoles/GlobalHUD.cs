using HarmonyLib;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.Enums;

namespace TownOfUsReworked.PlayerLayers.Roles.AllRoles
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class GlobalHUD
    {
        private static Sprite Lock = TownOfUsReworked.Lock;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || GameStates.IsLobby)
                return;

            __instance.KillButton.gameObject.SetActive(false);
            __instance.ReportButton.gameObject.SetActive(GameStates.IsRoaming);
            var role = Role.GetRole(PlayerControl.LocalPlayer);

            if (Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) && GameStates.IsInGame)
            {
                Sprite Vent;

                if (PlayerControl.LocalPlayer.Is(Faction.Intruder))
                    Vent = TownOfUsReworked.IntruderVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Syndicate))
                    Vent = TownOfUsReworked.SyndicateVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Crew))
                    Vent = TownOfUsReworked.CrewVent;
                else if (PlayerControl.LocalPlayer.Is(Faction.Neutral))
                    Vent = TownOfUsReworked.NeutralVent;
                else
                    Vent = __instance.ImpostorVentButton.graphic.sprite;

                __instance.ImpostorVentButton.graphic.sprite = Vent;
            }

            if (MapBehaviour.Instance)
            {
                __instance.ImpostorVentButton.gameObject.SetActive(false);
                __instance.ReportButton.gameObject.SetActive(false);
            }
            else
            {
                __instance.ImpostorVentButton.gameObject.SetActive(GameStates.IsRoaming);
                __instance.ReportButton.gameObject.SetActive(GameStates.IsRoaming);
            }

            if (MeetingHud.Instance != null)
            {
                foreach (var player in MeetingHud.Instance.playerStates)
                {
                    if (player.NameText != null && role.Player.PlayerId == player.TargetPlayerId)
                        player.NameText.color = role.Color;
                }
            }

            if (HudManager.Instance != null && HudManager.Instance.Chat != null)
            {
                foreach (var bubble in HudManager.Instance.Chat.chatBubPool.activeChildren)
                {
                    if (bubble.Cast<ChatBubble>().NameText != null && role.Player.Data.PlayerName == bubble.Cast<ChatBubble>().NameText.text)
                        bubble.Cast<ChatBubble>().NameText.color = role.Color;
                }
            }
        }
    }
}