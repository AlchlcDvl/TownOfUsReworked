using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using AmongUs.GameOptions;

namespace TownOfUsReworked.Patches
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.Start))]
    public static class KillButtonAwake
    {
        public static void Prefix(KillButton __instance)
        {
            __instance.transform.Find("Text_TMP").gameObject.SetActive(false);
        }
    }
    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillButtonSprite
    {
        /*public static Sprite Kill;

        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null)
                return;

            if (!Kill)
                Kill = __instance.KillButton.graphic.sprite;

            if (Utils.CanVent(PlayerControl.LocalPlayer, PlayerControl.LocalPlayer.Data) && GameStates.IsInGame)
                __instance.ImpostorVentButton.transform.localPosition = new Vector3(-2f, 0f, 0f);

            var keyInt = Input.GetKeyInt(KeyCode.Q);
            var controller = ConsoleJoystick.player.GetButtonDown(8);

            if (keyInt && __instance.KillButton != null)
                __instance.KillButton.DoClick();
        }*/

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (!GameStates.IsInGame)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (GameStates.IsHnS)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsImpostor());
                    return;
                }

                var ghostRole = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Revealer))
                {
                    var haunter = Role.GetRole<Revealer>(PlayerControl.LocalPlayer);

                    if (!haunter.Caught)
                        ghostRole = true;
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                {
                    var phantom = Role.GetRole<Phantom>(PlayerControl.LocalPlayer);

                    if (!phantom.Caught)
                        ghostRole = true;
                }

                HudManager.Instance.AbilityButton.gameObject.SetActive(!ghostRole && !MeetingHud.Instance);
            }
        }
    }
}
