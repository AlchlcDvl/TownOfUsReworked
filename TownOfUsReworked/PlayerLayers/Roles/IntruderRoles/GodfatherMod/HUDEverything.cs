using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.DisguiserMod;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDEverything
    {
        public static Sprite Promote => TownOfUsReworked.Placeholder;
        public static Sprite Blackmail => TownOfUsReworked.BlackmailSprite;
        public static Sprite Camouflage => TownOfUsReworked.Camouflage;
        public static Sprite MeasureSprite => TownOfUsReworked.MeasureSprite;
        public static Sprite DisguiseSprite => TownOfUsReworked.DisguiseSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Godfather))
                return;

            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            var notImp = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Intruder)).ToList();
            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.IntKillCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notImp);
            var renderer2 = role.KillButton.graphic;
            
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

            if (!role.HasDeclared)
            {
                if (role.DeclareButton == null)
                {
                    role.DeclareButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.DeclareButton.graphic.enabled = true;
                    role.DeclareButton.graphic.sprite = Promote;
                    role.DeclareButton.gameObject.SetActive(false);
                }

                var Imp = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Intruder)).ToList();

                role.DeclareButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && !role.HasDeclared);
                Utils.SetTarget(ref role.ClosestPlayer, role.DeclareButton, Imp);

                var renderer = role.DeclareButton.graphic;
                
                if (role.ClosestPlayer != null && !role.DeclareButton.isCoolingDown && !role.Disguised)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }

            if (role.FormerRole == null)
                return;

            var formerRole = role.FormerRole.RoleType;

            if (formerRole == RoleEnum.Blackmailer)
            {
                if (role.BlackmailButton == null)
                {
                    role.BlackmailButton = Object.Instantiate(__instance.KillButton, __instance.UseButton.transform.parent);
                    role.BlackmailButton.graphic.enabled = true;
                    role.BlackmailButton.graphic.sprite = Blackmail;
                    role.BlackmailButton.gameObject.SetActive(false);
                }

                role.BlackmailButton.SetCoolDown(role.BlackmailTimer(), CustomGameOptions.BlackmailCd);
                role.BlackmailButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
                var notBlackmailed = PlayerControl.AllPlayerControls.ToArray().Where(player => role.Blackmailed?.PlayerId != player.PlayerId).ToList();
                Utils.SetTarget(ref role.ClosestPlayer, role.BlackmailButton, notBlackmailed);

                var renderer = role.BlackmailButton.graphic;
                
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
            }
            else if (formerRole == RoleEnum.Consigliere)
            {
                if (role.CamouflageButton == null)
                {
                    role.CamouflageButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.CamouflageButton.graphic.sprite = Camouflage;
                    role.CamouflageButton.graphic.enabled = true;
                    role.CamouflageButton.gameObject.SetActive(false);
                }

                if (role.CamoEnabled)
                    role.CamouflageButton.SetCoolDown(role.CamoTimeRemaining, CustomGameOptions.CamouflagerDuration);
                else
                    role.CamouflageButton.SetCoolDown(role.CamouflageTimer(), CustomGameOptions.CamouflagerCd);
            
                var renderer = role.CamouflageButton.graphic;

                if (!role.Camouflaged && !role.CamouflageButton.isCoolingDown)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
            else if (formerRole == RoleEnum.Disguiser)
            {
                if (role.DisguiseButton == null)
                {
                    role.DisguiseButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.DisguiseButton.graphic.enabled = true;
                    role.DisguiseButton.graphic.sprite = MeasureSprite;
                    role.DisguiseButton.gameObject.SetActive(false);

                    if (role.DisguiseButton.graphic.sprite != MeasureSprite && role.DisguiseButton.graphic.sprite != DisguiseSprite)
                        role.DisguiseButton.graphic.sprite = MeasureSprite;
                }

                var targets = PlayerControl.AllPlayerControls.ToArray().ToList();

                if (CustomGameOptions.DisguiseTarget == DisguiserTargets.Intruders)
                    targets = targets.Where(x => x.Is(Faction.Intruder)).ToList();
                else if (CustomGameOptions.DisguiseTarget == DisguiserTargets.NonIntruders)
                    targets = targets.Where(x => !x.Is(Faction.Intruder)).ToList();

                role.DisguiseButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));

                if (role.DisguiseButton.graphic.sprite == MeasureSprite)
                    role.DisguiseButton.SetCoolDown(0f, 1f);
                else
                {
                    if (role.Disguised)
                        role.DisguiseButton.SetCoolDown(role.DisguiseTimeRemaining, CustomGameOptions.DisguiseDuration);
                    else
                        role.DisguiseButton.SetCoolDown(role.DisguiseTimer(), CustomGameOptions.DisguiseCooldown);
                }

                var renderer = role.DisguiseButton.graphic;
                
                if (role.ClosestPlayer != null && !role.DisguiseButton.isCoolingDown && !role.Disguised)
                {
                    renderer.color = Palette.EnabledColor;
                    renderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    renderer.color = Palette.DisabledClear;
                    renderer.material.SetFloat("_Desat", 1f);
                }
            }
        }
    }
}