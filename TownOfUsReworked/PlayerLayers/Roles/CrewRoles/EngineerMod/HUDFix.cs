using System.Linq;
using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.EngineerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class HUDFix
    {
        private static Sprite Fix => TownOfUsReworked.EngineerFix;
        
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                return;

            if (__instance.KillButton == null)
                return;

            var role = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
            
            __instance.KillButton.graphic.sprite = Fix;
            __instance.KillButton.SetCoolDown(0f, 10f);
            __instance.KillButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && __instance.UseButton.isActiveAndEnabled && !MeetingHud.Instance && !LobbyBehaviour.Instance);

            if (!ShipStatus.Instance)
                return;

            var system = ShipStatus.Instance.Systems[SystemTypes.Sabotage].Cast<SabotageSystemType>();

            if (system == null)
                return;

            var camouflager = Role.GetRoleValue<Camouflager>(RoleEnum.Camouflager);
            var concealer = Role.GetRoleValue<Concealer>(RoleEnum.Concealer);
            var shapeshifter = Role.GetRoleValue<Shapeshifter>(RoleEnum.Shapeshifter);

            var specials = system.specials.ToArray();
            var dummyActive = system.dummy.IsActive;
            var active = specials.Any(s => s.IsActive) || camouflager.Camouflaged || concealer.Concealed || shapeshifter.Shapeshifted;
            var renderer = __instance.KillButton.graphic;
            
            if (active && !dummyActive && !role.UsedThisRound && __instance.KillButton.enabled)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                return;
            }

            renderer.color = Palette.DisabledClear;
            renderer.material.SetFloat("_Desat", 1f);
        }
    }
}