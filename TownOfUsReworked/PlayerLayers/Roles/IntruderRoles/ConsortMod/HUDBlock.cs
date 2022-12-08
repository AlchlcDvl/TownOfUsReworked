using HarmonyLib;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsortMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HUDBlock
    {
        public static Sprite Roleblock => TownOfUsReworked.Placeholder;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1)
                return;

            if (PlayerControl.LocalPlayer == null)
                return;

            if (PlayerControl.LocalPlayer.Data == null)
                return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Consort))
                return;

            var isDead = PlayerControl.LocalPlayer.Data.IsDead;

            if (isDead)
                return;
                
            var role = Role.GetRole<Consort>(PlayerControl.LocalPlayer);

            if (role.RoleblockButton == null)
            {
                role.RoleblockButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RoleblockButton.graphic.enabled = true;
                role.RoleblockButton.GetComponent<AspectPosition>().DistanceFromEdge = TownOfUsReworked.BelowVentPosition;
                role.RoleblockButton.gameObject.SetActive(false);
            }

            role.RoleblockButton.GetComponent<AspectPosition>().Update();
            role.RoleblockButton.graphic.sprite = Roleblock;
            role.RoleblockButton.gameObject.SetActive(!PlayerControl.LocalPlayer.Data.IsDead && !MeetingHud.Instance);
            Utils.SetTarget(ref role.ClosestPlayer, role.RoleblockButton);
        }
    }
}