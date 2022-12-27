using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.IO;
using Rewired.UI.ControlMapper;

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
        private static Sprite Shift => TownOfUsReworked.Shift;
        private static Sprite Rewind => TownOfUsReworked.Rewind;
        private static Sprite Medic => TownOfUsReworked.MedicSprite;
        private static Sprite Seer => TownOfUsReworked.SeerSprite;
        private static Sprite Douse => TownOfUsReworked.DouseSprite;
        private static Sprite Revive => TownOfUsReworked.ReviveSprite;
        private static Sprite Alert => TownOfUsReworked.AlertSprite;
        private static Sprite Remember => TownOfUsReworked.RememberSprite;
        private static Sprite Eat => TownOfUsReworked.CannibalEat;
        private static Sprite Track => TownOfUsReworked.TrackSprite;
        private static Sprite Transport => TownOfUsReworked.TransportSprite;
        private static Sprite Mediate => TownOfUsReworked.MediateSprite;
        private static Sprite Vest => TownOfUsReworked.VestSprite;
        private static Sprite Protect => TownOfUsReworked.ProtectSprite;
        private static Sprite Infect => TownOfUsReworked.InfectSprite;
        private static Sprite Bug => TownOfUsReworked.BugSprite;
        private static Sprite Examine => TownOfUsReworked.ExamineSprite;
        private static Sprite Button => TownOfUsReworked.ButtonSprite;
        private static Sprite Fix => TownOfUsReworked.EngineerFix;
        private static Sprite Shoot => TownOfUsReworked.ShootSprite;
        private static Sprite Maul => TownOfUsReworked.MaulSprite;
        private static Sprite Obliterate => TownOfUsReworked.ObliterateSprite;
        private static Sprite Assault => TownOfUsReworked.AssaultSprite;
        private static Sprite EraseData => TownOfUsReworked.EraseDataSprite;
        private static Sprite Disguise => TownOfUsReworked.DisguiseSprite;
        private static Sprite Placeholder => TownOfUsReworked.Placeholder;
        private static Sprite Clear => TownOfUsReworked.Clear;
        private static Sprite Kill;

        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null)
                return;

            if (!Kill)
                Kill = __instance.KillButton.graphic.sprite;

            var flag = false;

            if (PlayerControl.LocalPlayer.Is(RoleEnum.TimeLord))
            {
                __instance.KillButton.graphic.sprite = Rewind;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Shifter))
            {
                __instance.KillButton.graphic.sprite = Shift;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
            {
                __instance.KillButton.graphic.sprite = Seer;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medic))
            {
                __instance.KillButton.graphic.sprite = Medic;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                __instance.KillButton.graphic.sprite = Douse;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Altruist))
            {
                __instance.KillButton.graphic.sprite = Revive;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                __instance.KillButton.graphic.sprite = Alert;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac))
            {
                __instance.KillButton.graphic.sprite = Remember;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                __instance.KillButton.graphic.sprite = Track;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                __instance.KillButton.graphic.sprite = Transport;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                __instance.KillButton.graphic.sprite = Mediate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                __instance.KillButton.graphic.sprite = Vest;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                __instance.KillButton.graphic.sprite = Protect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                __instance.KillButton.graphic.sprite = Infect;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
            {
                __instance.KillButton.graphic.sprite = Fix;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Operative))
            {
                __instance.KillButton.graphic.sprite = Bug;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                __instance.KillButton.graphic.sprite = Examine;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Vigilante))
            {
                __instance.KillButton.graphic.sprite = Shoot;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
            {
                var sk = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);
                __instance.KillButton.gameObject.SetActive(sk.Lusted);
                __instance.KillButton.graphic.sprite = Placeholder;

                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
            {
                __instance.KillButton.graphic.sprite = Obliterate;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
            {
                __instance.KillButton.graphic.sprite = Assault;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
            {
                __instance.KillButton.graphic.sprite = EraseData;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cannibal))
            {
                __instance.KillButton.graphic.sprite = Eat;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Dracula))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Murderer))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Escort))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Cryomaniac))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
            {
                __instance.KillButton.graphic.sprite = Placeholder;
                flag = true;
            }
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                __instance.KillButton.graphic.sprite = Maul;
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

                flag = (PlayerControl.LocalPlayer.Is(Faction.Intruder) && !PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) |
                    PlayerControl.LocalPlayer.Is(Faction.Syndicate);

                if (flag) 
                    __instance.KillButton.buttonLabelText.text = "Kill";
            }

            /*string key = File.ReadAllText(Application.persistentDataPath + "\\ToUKeybind.txt");
            KeyCode KeyCode = (KeyCode) System.Enum.Parse(typeof(KeyCode), key);
            var keyInt = Input.GetKeyInt(KeyCode);*/

            var keyInt = Input.GetKeyInt(KeyCode.Q);
            var controller = ConsoleJoystick.player.GetButtonDown(8);

            if (keyInt | controller && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)
                __instance.KillButton.DoClick();
        }
    }
}
