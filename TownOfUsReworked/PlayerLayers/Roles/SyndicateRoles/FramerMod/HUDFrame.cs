using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using UnityEngine;

namespace TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.FramerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDFramer
    {
        public static Sprite FrameSprite => TownOfUsReworked.Placeholder;
        public static Sprite Kill => TownOfUsReworked.SyndicateKill;

        public static void Postfix(HudManager __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Framer))
                return;

            var role = Role.GetRole<Framer>(PlayerControl.LocalPlayer);

            if (role.FrameButton == null)
            {
                role.FrameButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.FrameButton.graphic.enabled = true;
                role.FrameButton.graphic.sprite = FrameSprite;
                role.FrameButton.gameObject.SetActive(false);
            }

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.graphic.sprite = Kill;
                role.KillButton.gameObject.SetActive(false);
            }

            role.FrameButton.gameObject.SetActive(!MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead && !LobbyBehaviour.Instance);
            var notFramed = PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Framed.Contains(x.PlayerId) && !x.Is(Faction.Syndicate)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, role.FrameButton, notFramed);
            role.FrameButton.SetCoolDown(role.FrameTimer(), CustomGameOptions.FrameCooldown);

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

            var renderer2 = role.FrameButton.graphic;
            
            if (role.ClosestPlayer != null && !role.FrameButton.isCoolingDown)
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