using HarmonyLib;
using UnityEngine;

namespace TownOfUs
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
        private static Sprite Shift => TownOfUs.Shift;
        private static Sprite Rewind => TownOfUs.Rewind;
        private static Sprite Medic => TownOfUs.MedicSprite;
        private static Sprite Seer => TownOfUs.SeerSprite;
        private static Sprite Douse => TownOfUs.DouseSprite;
        private static Sprite Revive => TownOfUs.ReviveSprite;
        private static Sprite Alert => TownOfUs.AlertSprite;
        private static Sprite Remember => TownOfUs.RememberSprite;
        private static Sprite Eat => TownOfUs.CannibalEat;
        private static Sprite Track => TownOfUs.TrackSprite;
        private static Sprite Transport => TownOfUs.TransportSprite;
        private static Sprite Mediate => TownOfUs.MediateSprite;
        private static Sprite Vest => TownOfUs.VestSprite;
        private static Sprite Protect => TownOfUs.ProtectSprite;
        private static Sprite Infect => TownOfUs.InfectSprite;
        private static Sprite Bug => TownOfUs.BugSprite;
        private static Sprite Examine => TownOfUs.ExamineSprite;
        private static Sprite Button => TownOfUs.ButtonSprite;
        private static Sprite Fix => TownOfUs.EngineerFix;
        private static Sprite Shoot => TownOfUs.ShootSprite;
        private static Sprite Maul => TownOfUs.MaulSprite;
        private static Sprite Obliterate => TownOfUs.ObliterateSprite;
        private static Sprite Assault => TownOfUs.AssaultSprite;
        private static Sprite EraseData => TownOfUs.EraseDataSprite;
        private static Sprite Disguise => TownOfUs.DisguiseSprite;
        private static Sprite Kill;


        public static void Postfix(HudManager __instance)
        {
            if (__instance.KillButton == null) return;

            if (!Kill) Kill = __instance.KillButton.graphic.sprite;

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
            else if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                __instance.KillButton.graphic.sprite = Maul;
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
            /*else if (PlayerControl.LocalPlayer.Is(RoleEnum.Dracula))
            {
                __instance.KillButton.graphic.sprite = Eat;
                flag = true;
            }*/
            else
            {
                __instance.KillButton.graphic.sprite = Kill;
                __instance.KillButton.buttonLabelText.gameObject.SetActive(true);

                if (PlayerControl.LocalPlayer.Is(Faction.Intruders) && !PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) 
                    __instance.KillButton.buttonLabelText.text = "Kill";

                flag =  PlayerControl.LocalPlayer.Is(Faction.Intruders) && !PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner);
            }

            var keyInt = Input.GetKeyInt(KeyCode.Q);
            var controller = ConsoleJoystick.player.GetButtonDown(8);
            if (keyInt | controller && __instance.KillButton != null && flag && !PlayerControl.LocalPlayer.Data.IsDead)__instance.KillButton.DoClick();
        }
    }
}
