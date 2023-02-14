using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using TownOfUsReworked.Lobby.CustomOption;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using System.Linq;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.BomberMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBomb
    {
        public static Sprite Bomb => TownOfUsReworked.PlantSprite;
        public static Sprite Detonate => TownOfUsReworked.DetonateSprite;
        public static Sprite Kill => TownOfUsReworked.SyndicateKill;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Bomber))
                return;

            var role = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);

            if (role.BombButton == null)
            {
                role.BombButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.BombButton.graphic.enabled = true;
                role.BombButton.graphic.sprite = Bomb;
                role.BombButton.gameObject.SetActive(false);
            }

            role.BombButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.BombButton.SetCoolDown(role.BombTimer(), CustomGameOptions.BombCooldown);

            var renderer = role.BombButton.graphic;
            
            if (!role.BombButton.isCoolingDown)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }

            if (role.DetonateButton == null)
            {
                role.DetonateButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.DetonateButton.graphic.enabled = true;
                role.DetonateButton.graphic.sprite = Detonate;
                role.DetonateButton.gameObject.SetActive(false);
            }

            role.DetonateButton.gameObject.SetActive(Utils.SetActive(role.Player, __instance));
            role.DetonateButton.SetCoolDown(role.DetonateTimer(), CustomGameOptions.DetonateCooldown);
            var renderer2 = role.DetonateButton.graphic;
            
            if (!role.DetonateButton.isCoolingDown && role.Bombs.Count > 0)
            {
                renderer2.color = Palette.EnabledColor;
                renderer2.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer2.color = Palette.DisabledClear;
                renderer2.material.SetFloat("_Desat", 1f);
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
            var renderer3 = role.KillButton.graphic;
            
            if (role.ClosestPlayer != null && !role.KillButton.isCoolingDown)
            {
                renderer3.color = Palette.EnabledColor;
                renderer3.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer3.color = Palette.DisabledClear;
                renderer3.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
