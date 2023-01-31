using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.IO;
using Rewired.UI.ControlMapper;
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

    [HarmonyPatch(typeof(ControlMapper), nameof(ControlMapper.OnKeyboardElementAssignmentPollingWindowUpdate))]
    public class KillKeybind
    {
        [HarmonyPostfix]
        public static void postfix(ControlMapper __instance)
        {
            if (__instance.pendingInputMapping.actionName == "Kill")
            {
                string newbind = __instance.pendingInputMapping.elementName;

                if (newbind != "None")
                    File.WriteAllTextAsync(Application.persistentDataPath + "\\ToUKeybind.txt", newbind.Replace(" ", string.Empty));
            } 
        }
    }
    
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class KillButtonSprite
    {
        public static Sprite Disguise => TownOfUsReworked.DisguiseSprite;
        public static Sprite Placeholder => TownOfUsReworked.Placeholder;
        public static Sprite Clear => TownOfUsReworked.Clear;
        public static Sprite Bite => TownOfUsReworked.Placeholder;
        public static Sprite Kill;

        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null)
                return;

            if (!Kill)
                Kill = __instance.KillButton.graphic.sprite;

            var flag = false;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Dracula))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Troll))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);

                flag = (PlayerControl.LocalPlayer.Is(Faction.Intruder) && !PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) ||
                    PlayerControl.LocalPlayer.Is(Faction.Syndicate);

                if (flag) 
                    __instance.KillButton.buttonLabelText.text = "Kill";
            }

            /*string key = File.ReadAllText(Application.persistentDataPath + "\\ToUKeybind.txt");
            KeyCode KeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), key);
            var keyInt = Input.GetKeyInt(KeyCode);*/

            var keyInt = Input.GetKeyInt(KeyCode.Q);
            var controller = ConsoleJoystick.player.GetButtonDown(8);

            if (keyInt || controller && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.KillButton.DoClick();
        }

        [HarmonyPatch(typeof(AbilityButton), nameof(AbilityButton.Update))]
        class AbilityButtonUpdatePatch
        {
            static void Postfix()
            {
                if (AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                {
                    HudManager.Instance.AbilityButton.gameObject.SetActive(false);
                    return;
                }
                else if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
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
