using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using UnityEngine;
using System.Linq;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.PoisonerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDPoison
    {
        public static Sprite PoisonSprite => TownOfUsReworked.PoisonSprite;
        public static Sprite PoisonedSprite => TownOfUsReworked.PoisonedSprite;
        public static Sprite Kill => TownOfUsReworked.SyndicateKill;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Poisoner))
                return;

            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);

            if (role.PoisonButton == null)
            {
                role.PoisonButton = Object.Instantiate(__instance.KillButton, HudManager.Instance.transform);
                role.PoisonButton.graphic.enabled = true;
                role.PoisonButton.graphic.sprite = PoisonSprite;
                role.PoisonButton.gameObject.SetActive(false);
            }

            role.PoisonButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            var notSyn = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate) && x != role.PoisonedPlayer).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, notSyn);

            if (role.ClosestPlayer != null)
                role.ClosestPlayer.myRend().material.SetColor("_OutlineColor", role.Color);

            if (role.Poisoned)
            {
                role.PoisonButton.graphic.sprite = PoisonedSprite;
                role.Poison();
                role.PoisonButton.SetCoolDown(role.TimeRemaining, CustomGameOptions.PoisonDuration);
            }
            else
            {
                role.PoisonButton.graphic.sprite = PoisonSprite;

                if (role.PoisonedPlayer && role.PoisonedPlayer != PlayerControl.LocalPlayer)
                    role.PoisonKill();

                role.PoisonButton.SetCoolDown(role.PoisonTimer(), CustomGameOptions.PoisonCd);
                role.PoisonedPlayer = PlayerControl.LocalPlayer; //Only do this to stop repeatedly trying to re-kill poisoned player. null didn't work for some reason
            }
                        
            if (role.ClosestPlayer != null && !role.PoisonButton.isCoolingDown && !role.Poisoned)
            {
                role.PoisonButton.graphic.color = Palette.EnabledColor;
                role.PoisonButton.graphic.material.SetFloat("_Desat", 0f);
            }
            else
            {
                role.PoisonButton.graphic.color = Palette.DisabledClear;
                role.PoisonButton.graphic.material.SetFloat("_Desat", 1f);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.graphic.sprite = Kill;
                role.KillButton.gameObject.SetActive(false);
            }

            role.KillButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance) && Role.SyndicateHasChaosDrive);
            role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.ChaosDriveKillCooldown);
            var notSyndicate = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(Faction.Syndicate)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, notSyndicate);
            var renderer = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
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
