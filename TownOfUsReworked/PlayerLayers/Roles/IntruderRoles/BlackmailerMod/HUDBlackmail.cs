using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using TownOfUsReworked.Classes;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.BlackmailerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlackmail
    {
        public static Sprite Blackmail => TownOfUsReworked.BlackmailSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Blackmailer))
                return;

            var role = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);

            if (role.BlackmailButton == null)
            {
                role.BlackmailButton = Object.Instantiate(__instance.KillButton, __instance.UseButton.transform.parent);
                role.BlackmailButton.graphic.enabled = true;
                role.BlackmailButton.graphic.sprite = Blackmail;
                role.BlackmailButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();

            role.BlackmailButton.SetCoolDown(role.BlackmailTimer(), CustomGameOptions.BlackmailCd);
            role.BlackmailButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => role.Blackmailed?.PlayerId != player.PlayerId).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.BlackmailButton, notBlackmailed);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);

            if (role.Blackmailed != null && !role.Blackmailed.Data.IsDead && !role.Blackmailed.Data.Disconnected)
            {
                role.Blackmailed.myRend().material.SetFloat("_Outline", 1f);
                role.Blackmailed.myRend().material.SetColor("_OutlineColor", role.Color);

                if (role.Blackmailed.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && role.Blackmailed.GetCustomOutfitType() !=
                    CustomPlayerOutfitType.Invis)
                    role.Blackmailed.nameText().color = role.Color;
                else
                    role.Blackmailed.nameText().color = Color.clear;
            }

            var imps = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Data.IsImpostor() && player != role.Blackmailed).ToList();

            foreach (var imp in imps)
            {
                if ((imp.GetCustomOutfitType() == CustomPlayerOutfitType.Camouflage || imp.GetCustomOutfitType() == CustomPlayerOutfitType.Invis) && imp.nameText().color == role.Color)
                    imp.nameText().color = Color.clear;
                else if (imp.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && imp.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && imp.nameText().color == Color.clear)
                    imp.nameText().color = role.Color;
            }

            var renderer = role.BlackmailButton.graphic;
            var renderer2 = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.BlackmailButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
            }
        }
    }
}